using CSF.Logmgr;
using CSF.Plugin;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CSF.WPF.Core.Data.Plugin
{
    public static class PluginManager
    {
        private static readonly string PLUGINS_DIRECTORY = AppContext.BaseDirectory + "\\Plugins";
        private static readonly string PLUGINS_CONFIG_FILE = PLUGINS_DIRECTORY + "\\Plugins";


        public static ViewModel.DocumentsViewModel Documents { get; set; }
        public static MenuItem[] ImportList;
        public static MenuItem[] ExportList;
        public static MenuItem[] ConverterList;
        public static MenuItem[] SingleConverterList;
        public static MenuItem[] PluginList;
        public static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        public static IEnumerable<(Type,IPlugin)> CreateCommands(Assembly assembly)
        {
            int count = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    IPlugin result = Activator.CreateInstance(type) as IPlugin;
                    if (result != null)
                    {
                        count++;
                        yield return (type, result);
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
        public static void SwitchPlugin(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            using (var sr = new StreamReader(new FileStream(AppContext.BaseDirectory + "\\Plugins\\Plugins", FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split('|');
                    if (((sender as MenuItem).Header as string).Trim().Equals(line[1].Trim()))
                    {
                        sb.AppendLine(string.Format("{0}|{1}|{2}", (sender as MenuItem).IsChecked ? "0" : "1", line[1], line[2]));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("{0}|{1}|{2}", line[0], line[1], line[2]));
                    }
                }
            }
            File.Delete(AppContext.BaseDirectory + "\\Plugins\\Plugins");
            File.WriteAllText(AppContext.BaseDirectory + "\\Plugins\\Plugins", sb.ToString());
            MessageBox.Show("重启应用生效", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static IEnumerable<(Type,IPlugin)> GetPlugins()// 获取插件命令
        {
            var pluginList = new List<MenuItem>();
            using (var configReader = new StreamReader(new FileStream(PLUGINS_CONFIG_FILE, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                while (!configReader.EndOfStream)
                {
                    var line = configReader.ReadLine().Split("|");
                    var menu = new MenuItem
                    {
                        Header = line[1],
                        IsCheckable = true,
                        IsChecked = false
                    };
                    menu.Click += SwitchPlugin;
                    if (line[0].Trim().Equals("0"))
                    {
                        menu.IsChecked = true;
                        Logger.Info("Using Plugin {0}.", line[2]);
                        Assembly pluginAssembly = LoadPlugin(AppContext.BaseDirectory + "\\Plugins" + line[2]);

                        // Load commands from plugins.
                        int count = 0;
                        foreach (Type type in pluginAssembly.GetTypes())
                        {
                            if (typeof(IPlugin).IsAssignableFrom(type))
                            {
                                var result = Activator.CreateInstance(type) as IPlugin;
                                if (result != null)
                                {
                                    count++;
                                    yield return (type, result);
                                }
                            }
                        }

                        if (count == 0)
                        {
                            string availableTypes = string.Join(",", pluginAssembly.GetTypes().Select(t => t.FullName));
                            throw new ApplicationException(
                                $"Can't find any type which implements ICommand in {pluginAssembly} from {pluginAssembly.Location}.\n" +
                                $"Available types: {availableTypes}");
                        }
                    }
                    else
                    {
                        Logger.Info("Ignore Plugin {0}.", line[2]);
                    }
                    pluginList.Add(menu);
                }
            }
            PluginList = pluginList.ToArray();
        }

        public static void PluginInit()
        {
            Logger.Info("Plugin Initialization.");

            var importList = new List<MenuItem>();
            var exportList = new List<MenuItem>();
            var convertList = new List<MenuItem>();
            var singleConvertList = new List<MenuItem>();

            Logger.Trace("检查插件配置文件是否存在");
            if (!File.Exists(PLUGINS_CONFIG_FILE))
            {
                Logger.Trace("配置文件不存在 结束");
                return;
            }
            IEnumerable<(Type, IPlugin)> plugins;
            try
            {
                plugins = GetPlugins();
            }
            catch (Exception ex)
            {
                Logger.Warn("Load Plugin Fail. {0}", ex);
                return;
            }
            Logger.Trace("读取插件配置文件...");

            try
            {                
                Logger.Trace("读取插件配置文件...Done");
                Logger.Trace("开始遍历初始化启用的插件");
                // Output the loaded commands.
                foreach (var plugin in plugins)
                {
                    var type = plugin.Item1;
                    var command = plugin.Item2;
                    Logger.Info($"Find Command : {command.Name}\t - {command.Description}\t - {command.PluginType}");
                    if (command.PluginType == PluginType.CONVERTER) // 是转换器
                    {
                        var converter = command as ConvertPlugin;

                        var menu = new MenuItem();
                        menu.Header = converter.Name;
                        menu.ToolTip = converter.Description;
                        menu.Click += async (o, e) =>
                        {
                            try
                            {
                                Logger.Info("Converter Plugin is Running:\t{0}.", type);
                                await Documents.Converter(type.GetMethod(converter.ExecuteName).Invoke(type,
                                new object[] { Documents.SelectDocument.DocViewModel.TypeList }));
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn("Catched Plugin Exception \t:{0} |Exception:{1}", type, ex);
                                MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        };
                        convertList.Add(menu);

                        var singleMenu = new MenuItem();
                        singleMenu.Header = converter.SingleName;
                        singleMenu.ToolTip = converter.SingleDescription;
                        singleMenu.Click += async (o, e) =>
                        {
                            try
                            {
                                Logger.Info("Converter Plugin is Running(Single Item):\t{0}.", type);
                                var source = Documents.SelectDocument.DocViewModel.Data;
                                var newValues = new Model.Value[source.LabelValues.Length];
                                for (int i = 0; i < newValues.Length; i++)
                                {
                                    Model.Value value = source.LabelValues[i];
                                    object ret = type.GetMethod(converter.SingleExecuteName).Invoke(type,
                                        new object[] { value.ValueString });
                                    if (ret is string)
                                    {
                                        newValues[i] = new Model.Value(ret as string, value.ExtraString);
                                    }
                                    else if (ret is Task<string>)
                                    {
                                        newValues[i] = new Model.Value(await (ret as Task<string>).ConfigureAwait(true), value.ExtraString);
                                    }
                                }
                                Documents.SelectDocument.DocViewModel.Data.Changed(new Model.Label(source.LabelName, newValues));
                                Documents.SelectDocument.DocViewModel.Update(1);
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn("Catched Plugin Exception \tMethod:{0} |Exception:{1}", type, ex);
                                MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        };
                        singleConvertList.Add(singleMenu);
                    }
                    else if (command.PluginType == PluginType.IMPORTER || command.PluginType == PluginType.EXPORTER)
                    {
                        FileDialog fileDialog;
                        var converter = command as FilePlugin;

                        if (converter.PluginType == PluginType.IMPORTER)// 是导入方法
                        {
                            fileDialog = new OpenFileDialog();
                            var menu = new MenuItem();
                            menu.Header = converter.Name;
                            menu.ToolTip = converter.Description;
                            fileDialog.Filter = converter.Filter;
                            menu.Click += async (o, e) =>
                            {
                                Logger.Info("Importer Plugin is Running:\t{0}.", type);
                                if (fileDialog.ShowDialog() ?? false)
                                {
                                    try
                                    {
                                        Logger.Info("Plugin : Selected File :\t{0}.", fileDialog.FileName);
                                        object ret = type.GetMethod(converter.ExecuteName).Invoke(type, new object[] { fileDialog.FileName });
                                        if (ret is Model.File)
                                            Documents.Import(ret as Model.File);
                                        else if (ret is Task<Model.File>)
                                            Documents.Import(await (ret as Task<Model.File>).ConfigureAwait(true));
                                        else
                                        {
                                            Logger.Warn("Plugin : Unknow Type.");
                                            MessageBox.Show("操作未成功", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Warn("Catched Plugin Exception \tMethod:{0} |Exception:{1}", type, ex);
                                        MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    Logger.Info("Plugin : Cancel.");
                                    MessageBox.Show("操作取消", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            };
                            importList.Add(menu);
                        }
                        else// 是导出方法
                        {
                            fileDialog = new SaveFileDialog();
                            var menu = new MenuItem();
                            menu.Header = converter.Name;
                            menu.ToolTip = converter.Description;
                            fileDialog.Filter = converter.Filter;
                            menu.Click += async (o, e) =>
                            {
                                Logger.Info("Exporter Plugin is Running:\t{0}.", type);
                                if (fileDialog.ShowDialog() ?? false)
                                {
                                    Logger.Info("Plugin : Selected File :\t{0}.", fileDialog.FileName);
                                    try
                                    {
                                        object methodReturn = type.GetMethod(converter.ExecuteName).Invoke(type, new object[] { Documents.SelectDocument.DocViewModel.TypeList, fileDialog.FileName });
                                        if (methodReturn is bool
                                            ? (bool)methodReturn
                                            : methodReturn is Task<bool>
                                                ? await (methodReturn as Task<bool>).ConfigureAwait(true)
                                                : false)
                                        {
                                            Logger.Info("Plugin : DONE.");
                                            MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                        }
                                        else
                                        {
                                            Logger.Info("Plugin : FAIL.");
                                            MessageBox.Show("操作失败", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Warn("Catched Plugin Exception \tMethod:{0} |Exception:{1}", type, ex);
                                        MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    Logger.Info("Plugin : Cancel.");
                                    MessageBox.Show("操作取消", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            };
                            exportList.Add(menu);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Warn("Plugin Initializated Fail. {0}", ex);
            }
            ImportList = importList.ToArray();
            ExportList = exportList.ToArray();
            ConverterList = convertList.ToArray();
            SingleConverterList = singleConvertList.ToArray();
        }

    }
}
