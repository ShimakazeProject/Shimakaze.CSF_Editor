using CSF.Logmgr;
using CSF.Plugin;
using CSF.WPF.Core.Data;
using CSF.WPF.Core.View;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Json;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CSF.WPF.Core.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        internal const string PLUGIN_DIRECTORY_NAME = "\\Plugins";

        private CsfDoc document;
        private List<MenuItem> importList;
        private List<MenuItem> exportList;
        private List<MenuItem> converterList;
        private List<MenuItem> singleConverterList;
        private List<MenuItem> pluginList;

        public MainWindowViewModel()
        {
            Document = new CsfDoc();
            Documents = new DocumentsViewModel();

            PluginInit();
        }
        private void PluginInit()
        {
            var importList = new List<MenuItem>();
            var exportList = new List<MenuItem>();
            var convertList = new List<MenuItem>();
            var singleConvertList = new List<MenuItem>();
            var pluginList = new List<MenuItem>();

            try
            {
                Logger.Info("Plugin Initialization.");
                // Load commands from plugins.

                List<(Type, IPlugin)> plugins = new List<(Type, IPlugin)>();
                using (var configReader = new StreamReader(new FileStream(AppContext.BaseDirectory + PLUGIN_DIRECTORY_NAME + "\\Plugins", FileMode.Open, FileAccess.Read, FileShare.Read)))
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
                        menu.Click += Data.Plugin.PluginManager.SwitchPlugin;
                        if (line[0].Trim().Equals("0"))
                        {
                            menu.IsChecked = true;
                            Logger.Info("Using Plugin {0}.", line[2]);
                            Assembly pluginAssembly = Data.Plugin.PluginManager.LoadPlugin(AppContext.BaseDirectory + PLUGIN_DIRECTORY_NAME + line[2]);
                            plugins.AddRange(Data.Plugin.PluginManager.CreateCommands(pluginAssembly));
                        }
                        else
                        {
                            Logger.Info("Ignore Plugin {0}.", line[2]);
                        }
                        pluginList.Add(menu);
                    }
                }

                // Output the loaded commands.
                foreach (var plugin in plugins)
                {
                    var type = plugin.Item1;
                    var command = plugin.Item2;
                    Logger.Info($"Find Command : {command.Name}\t - {command.Description}");
                    if (command.PluginType == PluginType.CONVERTER) // 是转换器
                    {
                        var converter = command as ConvertPlugin;

                        var menu = new MenuItem();
                        menu.Header = converter.Name;
                        menu.ToolTip = converter.Description;
                        menu.Click += async (o, e) =>
                        {
                            Logger.Info("Converter Plugin is Running:\t{0}.", type);
                            object ret = type.GetMethod(converter.ExecuteName).Invoke(type,
                                new object[] { (Document.DataContext as CsfDocViewModel).TypeList });
                            try
                            {
                                if (ret is Model.File)
                                {
                                    var typeSet = new Model.TypeSet();
                                    typeSet.MakeType(ret as Model.File);
                                    (Document.DataContext as CsfDocViewModel).TypeList = typeSet;
                                    Logger.Info("Plugin : DONE.");
                                }
                                else if (ret is Task<Model.File>)
                                {
                                    var typeSet = new Model.TypeSet();
                                    typeSet.MakeType(await (ret as Task<Model.File>).ConfigureAwait(true));
                                    (Document.DataContext as CsfDocViewModel).TypeList = typeSet;
                                    Logger.Info("Plugin : DONE.");
                                }
                                else
                                {
                                    Logger.Warn("Plugin : Unknow Type.");
                                    MessageBox.Show("操作未成功", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
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
                                var source = (Document.DataContext as CsfDocViewModel).Data;
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
                                (Document.DataContext as CsfDocViewModel).Data.Changed(new Model.Label(source.LabelName, newValues));
                                (Document.DataContext as CsfDocViewModel).Update(1);
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn("Catched Plugin Exception \tMethod:{0} |Exception:{1}", type, ex);
                                MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        };
                        singleConvertList.Add(menu);
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
                                        {
                                            var typeSet = new Model.TypeSet();
                                            typeSet.MakeType(ret as Model.File);
                                            (Document.DataContext as CsfDocViewModel).TypeList = typeSet;
                                            Logger.Info("Plugin : DONE.");
                                        }
                                        else if (ret is Task<Model.File>)
                                        {
                                            var typeSet = new Model.TypeSet();
                                            typeSet.MakeType(await (ret as Task<Model.File>).ConfigureAwait(true));
                                            (Document.DataContext as CsfDocViewModel).TypeList = typeSet;
                                            Logger.Info("Plugin : DONE.");
                                        }
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
                                        object methodReturn = type.GetMethod(converter.ExecuteName).Invoke(type, new object[] { (Document.DataContext as CsfDocViewModel).TypeList, fileDialog.FileName });
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
            ImportList = importList;
            ExportList = exportList;
            ConverterList = convertList;
            SingleConverterList = singleConvertList;
            PluginList = pluginList;
        }


        public bool SearchModeTitle { get; set; } = true;
        public bool SearchModeValue { get; set; }
        public bool SearchModeExtra { get; set; }
        public bool SearchModeFull { get; set; }
        public bool SearchModeRegex { get; set; }
        public bool SearchIgnoreCase { get; set; } = true;

        public CsfDoc Document
        {
            get => document; set
            {
                document = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Document)));
            }
        }
        public List<MenuItem> ImportList
        {
            get => importList; set
            {
                importList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImportList)));
            }
        }
        public List<MenuItem> ExportList
        {
            get => exportList; set
            {
                exportList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExportList)));
            }
        }
        public List<MenuItem> ConverterList
        {
            get => converterList; set
            {
                converterList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConverterList)));
            }
        }
        public List<MenuItem> SingleConverterList
        {
            get => singleConverterList; set
            {
                singleConverterList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SingleConverterList)));
            }
        }
        public List<MenuItem> PluginList
        {
            get => pluginList; set
            {
                pluginList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PluginList)));
            }
        }

        public DocumentsViewModel Documents { get; set; }

        public Command.RelayCommand OpenFileCommand => new Command.RelayCommand(OpenFile);
        public Command.RelayCommand MergeFileCommand => new Command.RelayCommand(MergeFile);
        public Command.RelayCommand SaveFileCommand => new Command.RelayCommand(SaveFile);
        public Command.RelayCommand SaveAsFileCommand => new Command.RelayCommand(SaveAsFile);

        public Command.RelayCommand CloseFileCommand => new Command.RelayCommand(CloseFile);
        public Command.RelayCommand<Window> ExitCommand => new Command.RelayCommand<Window>(Exit);

        public Command.RelayCommand AddLabelCommand => new Command.RelayCommand(AddLabel);
        public Command.RelayCommand RemoveLabelCommand => new Command.RelayCommand(RemoveLabel);
        public Command.RelayCommand ChangeLabelCommand => new Command.RelayCommand(ChangeLabel);
        public Command.RelayCommand<string> SearchCommand => new Command.RelayCommand<string>(Search);




        public event PropertyChangedEventHandler PropertyChanged;

        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "C&C Strings File(*.CSF)|*.csf"
            };
            if (ofd.ShowDialog() ?? false)
            {
                (Document.DataContext as CsfDocViewModel).Open(ofd.FileName);
            }
        }
        private void MergeFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "C&C Strings File(*.CSF)|*.csf"
            };
            if (MessageBox.Show("合并时是否允许覆盖已存在的标签?", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (ofd.ShowDialog() ?? false)
                {
                    var ret = (document.DataContext as CsfDocViewModel).Merge(ofd.FileName);
                    MessageBox.Show(string.Format("覆盖{0}个重复标签,添加{1}个标签,共{2}个标签", ret.Item1, ret.Item2, ret.Item1 + ret.Item2), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (ofd.ShowDialog() ?? false)
                {
                    var ret = (document.DataContext as CsfDocViewModel).Merge(ofd.FileName, false);
                    MessageBox.Show(string.Format("添加{1}个标签,且发现但未并入{0}个重复标签", ret.Item1, ret.Item2), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void SaveFile()
        {
            if (string.IsNullOrEmpty((document.DataContext as CsfDocViewModel).FilePath))
            {
                SaveAsFile();
            }
            else (document.DataContext as CsfDocViewModel).Save();
        }
        private void SaveAsFile()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "C&C Strings File(*.CSF)|*.csf"
            };
            if (sfd.ShowDialog() ?? false)
            {
                (document.DataContext as CsfDocViewModel).SaveAs(sfd.FileName);
            }
        }
        private void CloseFile()
        {
            (document.DataContext as CsfDocViewModel).TypeList = new Model.TypeSet();
        }
        private void AddLabel()
        {
            (document.DataContext as CsfDocViewModel).AddLabel();
        }
        private void RemoveLabel()
        {
            (document.DataContext as CsfDocViewModel).DropLabel();
        }
        private void ChangeLabel()
        {
            (document.DataContext as CsfDocViewModel).EditLabel();
        }
        private void Exit(Window window) => window.Close();
        private void Search(string s)
        {
            Data.SearchMode mode = SearchMode.None;
            if (SearchModeTitle) mode |= SearchMode.Label;
            if (SearchModeValue) mode |= SearchMode.Value;
            if (SearchModeExtra) mode |= SearchMode.Extra;
            if (SearchModeFull) mode |= SearchMode.Full;
            if (SearchModeRegex) mode |= SearchMode.Regex;
            if (SearchIgnoreCase) mode |= SearchMode.IgnoreCase;

            (document.DataContext as CsfDocViewModel).Search(s, mode);
        }


    }
}
