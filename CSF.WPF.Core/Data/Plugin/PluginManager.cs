using CSF.Logmgr;
using CSF.Plugin;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CSF.WPF.Core.Data.Plugin
{
    public static class PluginManager
    {
        private static readonly string PLUGINS_DIRECTORY = AppContext.BaseDirectory + "Plugins" + Path.DirectorySeparatorChar;
        private static readonly string PLUGINS_CONFIG_FILE = PLUGINS_DIRECTORY + "Plugins";


        public static ViewModel.DocumentsViewModel Documents { get; set; }
        public static MenuItem[] ImportList;
        public static MenuItem[] ExportList;
        public static MenuItem[] ConverterList;
        public static MenuItem[] SingleConverterList;
        public static MenuItem[] PluginList;
        public static Assembly LoadPlugin(string relativePath)
        {
            //// 导航到解决方案根目录
            //string root = Path.GetFullPath(Path.Combine(
            //    Path.GetDirectoryName(
            //        Path.GetDirectoryName(
            //            Path.GetDirectoryName(
            //                Path.GetDirectoryName(
            //                    Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(PLUGINS_DIRECTORY, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Logger.Info("[{0}]\tLoading commands from : {1}.", nameof(PluginManager), pluginLocation);
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        /// <summary>
        /// 插件开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SwitchPlugin(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            using (var sr = new StreamReader(new FileStream(PLUGINS_CONFIG_FILE, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split('|');
                    if (((sender as MenuItem).Header as string).Trim().Equals(line[1].Trim()))
                    {
                        Logger.Info("[{0}]\t{2}able Plugin {1}.", nameof(PluginManager), (sender as MenuItem).Header, (sender as MenuItem).IsChecked ? "En" : "Dis");
                        sb.AppendLine(string.Format("{0}|{1}|{2}", (sender as MenuItem).IsChecked ? "0" : "1", line[1], line[2]));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("{0}|{1}|{2}", line[0], line[1], line[2]));
                    }
                }
            }
            File.Delete(PLUGINS_CONFIG_FILE);
            File.WriteAllText(PLUGINS_CONFIG_FILE, sb.ToString());
            MessageBox.Show("重启应用生效", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static IEnumerable<(Type,IPlugin)> GetPluginCommands()// 获取插件命令
        {
            using (var configReader = new StreamReader(new FileStream(PLUGINS_CONFIG_FILE, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                var pluginList = new List<MenuItem>();
                while (!configReader.EndOfStream)
                {
                    var lineStr = configReader.ReadLine();
                    if (lineStr.Trim().Length == 0) continue;
                    var line = lineStr.Split("|");
                    var menu = new MenuItem
                    {
                        Header = line[1],
                        IsCheckable = true,
                        IsChecked = false
                    };
                    menu.Click += SwitchPlugin;
                    pluginList.Add(menu);
                    if (line[0].Trim().Equals("0"))
                    {
                        menu.IsChecked = true;
                        Logger.Info("[{0}]\tEnable Plugin : {1}.", nameof(PluginManager), line[1]);
                        Assembly pluginAssembly = LoadPlugin(line[2]);

                        // Load commands from plugins.
                        var types = pluginAssembly.GetTypes();
                        if (types.Length > 0)
                        {
                            foreach (Type type in pluginAssembly.GetTypes())
                            {
                                if (typeof(IPlugin).IsAssignableFrom(type) &&
                                    Activator.CreateInstance(type) is IPlugin result)
                                {
                                    Logger.Info("[{0}]\tFind Command : {1} \t{2}\t- {3}", nameof(PluginManager), result.PluginType, result.Name, result.Description);
                                    yield return (type, result);
                                }
                            }
                        }
                        else
                        {
                            string availableTypes = string.Join(",", pluginAssembly.GetTypes().Select(t => t.FullName));
                            Logger.Warn("[{0}]\tCan't find any type which implements {1} in {2} from {3}.",
                                nameof(PluginManager), nameof(IPlugin), pluginAssembly, pluginAssembly.Location);
                            Logger.Warn("[{0}]\tAvailable types: {1}.", nameof(PluginManager), availableTypes);
                            yield break;
                        }
                    }
                    else
                    {
                        Logger.Info("[{0}]\tDisable Plugin :{1}.", nameof(PluginManager), line[1]);
                    }
                }
                PluginList = pluginList.ToArray();
            }
        }

        static PluginManager()
        {
            Logger.Info("[{0}]\tInitialization.", nameof(PluginManager));
            var importList = new List<MenuItem>();
            var exportList = new List<MenuItem>();
            var convertList = new List<MenuItem>();
            var singleConvertList = new List<MenuItem>();

            IEnumerable<(Type, IPlugin)> plugins;
            try
            {
                Logger.Info("[{0}]\tLoading Plugins", nameof(PluginManager));
                plugins = GetPluginCommands();
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException fnfe)
                {
                    Logger.Warn("[{0}]\tCannot Find File {1}", nameof(PluginManager), fnfe.FileName);
                }
                Logger.Warn("[{0}]\tPlugins failed to load : {1}", nameof(PluginManager), ex);
                return;
            }

            try
            {
                Logger.Info("[{0}]\tCommands Initialization", nameof(PluginManager));
                // Output the loaded commands.
                foreach (var plugin in plugins)
                {
                    Logger.Info("[{0}]\tCommand Initialization: {1} \t{2}", nameof(PluginManager), plugin.Item2.PluginType, plugin.Item2.Name);
                    var menu = new MenuItem
                    {
                        Header = plugin.Item2.Name,
                        ToolTip = plugin.Item2.Description,
                    };
                    switch (plugin.Item2.PluginType)
                    {
                        case PluginType.EXPORTER:// 是导出方法
                            menu.CommandParameter = ((Type, FilePlugin))plugin;
                            menu.Command = PluginCommands.ExportCommand;
                            exportList.Add(menu);
                            break;
                        case PluginType.IMPORTER:// 是导入方法
                            menu.CommandParameter = ((Type, FilePlugin))plugin;
                            menu.Command = PluginCommands.ImportCommand;
                            importList.Add(menu);
                            break;
                        case PluginType.CONVERTER:// 是转换器
                            menu.CommandParameter = ((Type, ConvertPlugin))plugin;
                            menu.Command = PluginCommands.DocumentConvertCommand;
                            convertList.Add(menu);
                            singleConvertList.Add(new MenuItem
                            {
                                Header = (plugin.Item2 as ConvertPlugin).SingleName,
                                ToolTip = (plugin.Item2 as ConvertPlugin).SingleDescription,
                                CommandParameter = ((Type, ConvertPlugin))plugin,
                                Command = PluginCommands.LabelConvertCommand
                            });
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("[{0}]\tPlugin Initializated Fail. {1}", nameof(PluginManager), ex);
            }
            ImportList = importList.ToArray();
            ExportList = exportList.ToArray();
            ConverterList = convertList.ToArray();
            SingleConverterList = singleConvertList.ToArray();
        }



        /// <summary>
        /// 插件加载上下文
        /// </summary>
        protected class PluginLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver resolver;
            public PluginLoadContext(string pluginPath) => resolver = new AssemblyDependencyResolver(pluginPath);
            protected override Assembly Load(AssemblyName assemblyName)
            {
                string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
                return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
            }
            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                string libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
            }
        }
    }
}
