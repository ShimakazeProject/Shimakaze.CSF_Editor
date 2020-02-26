using CSF.Logmgr;
using CSF.WPF.Core.Data;
using CSF.WPF.Core.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CSF.WPF.Core.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private CsfDoc document;
        private List<MenuItem> importList;
        private List<MenuItem> exportList;
        private List<MenuItem> converterList;
        private List<MenuItem> singleConverterList;

        public MainWindowViewModel()
        {
            Document = new CsfDoc();
            MenuListInit();
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
                (document.DataContext as CsfDocViewModel).Open(ofd.FileName);
            }
        }
        private void MergeFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "C&C Strings File(*.CSF)|*.csf"
            };
            if (ofd.ShowDialog() ?? false)
            {
                (document.DataContext as CsfDocViewModel).Merge(ofd.FileName);
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

        private void MenuListInit()
        {
            var importList = new List<MenuItem>();
            var exportList = new List<MenuItem>();
            var convertList = new List<MenuItem>();
            var singleConvertList = new List<MenuItem>();

            var jsonString = new StringBuilder();
            {
                using var fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\MenuItem.json", FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    jsonString.Append(sr.ReadLine().Split("//", StringSplitOptions.RemoveEmptyEntries)[0]);
                }
            }
            var config = JsonValue.Parse(jsonString.ToString());
            foreach (JsonObject json in config as JsonArray)
            {
                var MenuItem = new MenuItem();

                var info = ReflectionInit(json);
                if (info is null)
                {
                    Logger.Warn("导入失败:{0}", json);
                    continue;
                }
                MenuItem.Header = info.Name.GetValue(info.Class) as string;
                if (json.TryGetValue("menu_type", out JsonValue menuType))
                {
                    bool isAsync = json.TryGetValue("isAsync", out JsonValue isAsyncJV) ? (bool)isAsyncJV : false;
                    FileDialog fileDialog;
                    if (((string)menuType).Equals("exporter", StringComparison.Ordinal))// 是导出方法
                    {
                        fileDialog = new SaveFileDialog();
                        fileDialog.Filter = json.TryGetValue("filter", out JsonValue filter) ? (string)filter : "All File(*.*)|*.*";
                        MenuItem.Click += async (o, e) =>
                        {
                            if (fileDialog.ShowDialog() ?? false)
                            {
                                var args = new object[] { (Document.DataContext as CsfDocViewModel).TypeList, fileDialog.FileName };
                                try
                                {
                                    object ret = info.Method.Invoke(info.Class, args);
                                    bool finished = ret.GetType() == typeof(bool)
                                        ? !(bool)ret
                                        : ret.GetType() == typeof(Task<bool>) ? !await (ret as Task<bool>).ConfigureAwait(true) : false;
                                    if (finished)
                                    {
                                        MessageBox.Show("操作未成功", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Warn("扩展程序错误:{0} |异常:{1}", info.Method, ex);
                                    MessageBox.Show(info.Method.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        };
                        exportList.Add(MenuItem);
                    }
                    else if (((string)menuType).Equals("importer", StringComparison.Ordinal))// 是导入方法
                    {
                        fileDialog = new OpenFileDialog();
                        fileDialog.Filter = json.TryGetValue("filter", out JsonValue filter) ? (string)filter : "All File(*.*)|*.*";
                        MenuItem.Click += async (o, e) =>
                        {
                            if (fileDialog.ShowDialog() ?? false)
                            {
                                var args = new object[] { fileDialog.FileName };
                                object ret = info.Method.Invoke(info.Class, args);
                                if (ret.GetType() == typeof(Model.File))
                                {
                                    var typeSet = new Model.TypeSet();
                                    typeSet.MakeType(ret as Model.File);
                                    (Document.DataContext as CsfDocViewModel).TypeList = typeSet;
                                }
                                else if (ret.GetType() == typeof(Task<Model.File>))
                                {
                                    var typeSet = new Model.TypeSet();
                                    typeSet.MakeType(await (ret as Task<Model.File>).ConfigureAwait(true));
                                    (Document.DataContext as CsfDocViewModel).TypeList = typeSet;
                                }
                                else
                                {
                                    MessageBox.Show("操作未成功", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        };
                        importList.Add(MenuItem);
                    }
                    else if(((string)menuType).Equals("converter", StringComparison.Ordinal))// 是转换器
                    {
                        var converter = info.Class.GetMethod(json["converter"]);
                        var menu = new MenuItem
                        {
                            Header = info.Name.GetValue(info.Class) as string
                        };
                        menu.Click += async (o, e) =>
                        {
                            var source = (Document.DataContext as CsfDocViewModel).Data;
                            var newValues = new Model.Value[source.LabelValues.Length];
                            for (int i = 0; i < newValues.Length; i++)
                            {
                                Model.Value value = source.LabelValues[i];
                                var args = new object[] { value.ValueString };
                                object ret = converter.Invoke(info.Class, args);
                                if (ret.GetType() == typeof(string) )
                                {
                                    newValues[i] = new Model.Value(ret as string, value.ExtraString);
                                }
                                else if (ret.GetType() == typeof(Task<string>))
                                {
                                    newValues[i] = new Model.Value(await (ret as Task<string>).ConfigureAwait(true), value.ExtraString);
                                }
                            }
                            (Document.DataContext as CsfDocViewModel).Data.Changed(new Model.Label(source.LabelName, newValues));
                            (Document.DataContext as CsfDocViewModel).Update(2);
                        };
                        MenuItem.Click += async (o, e) =>
                        {
                            var args = new object[] { (Document.DataContext as CsfDocViewModel).TypeList };
                            object ret = info.Method.Invoke(info.Class, args);
                            if (ret.GetType() == typeof(Model.File)|| ret.GetType() == typeof(Model.TypeSet))
                            {
                                var typeSet = new Model.TypeSet();
                                typeSet.MakeType(ret as Model.File);
                                (Document.DataContext as CsfDocViewModel).TypeList = typeSet;
                            }
                            else if (ret.GetType() == typeof(Task<Model.File>)|| ret.GetType() == typeof(Task<Model.TypeSet>))
                            {
                                var typeSet = new Model.TypeSet();
                                typeSet.MakeType(await (ret as Task<Model.File>).ConfigureAwait(true));
                                (Document.DataContext as CsfDocViewModel).TypeList = typeSet;
                            }
                            else
                            {
                                MessageBox.Show("操作未成功", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                        };
                        convertList.Add(MenuItem);
                        singleConvertList.Add(menu);
                    }
                    else Logger.Info("未知类型:{0}:{1}", menuType, json);
                }
            }

            ImportList = importList;
            ExportList = exportList;
            ConverterList = convertList;
            SingleConverterList = singleConvertList;
        }
        private ReflectionStruct ReflectionInit(JsonObject json)
        {
            var reflection = new ReflectionStruct();
            try
            {
                //获取并加载DLL类库中的程序集
                reflection.Assembly = Assembly.LoadFile(AppContext.BaseDirectory + json["target"]);
                //获取类的类型：必须使用名称空间.类名称
                reflection.Class = reflection.Assembly.GetType(json["class"]);
                //获取类的属性：使用属性名
                reflection.Name = reflection.Class.GetProperty(json["name"]);
                //获取类的成员方法：使用方法名
                reflection.Method = reflection.Class.GetMethod(json["method"]);
            }
            catch (Exception ex)
            {
                Logger.Warn("加载程序集发生异常:{0}", ex);
                return null;
            }
            return reflection;
        }

    }
}
