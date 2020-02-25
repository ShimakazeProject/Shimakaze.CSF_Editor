using CSF.Logmgr;
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

        public MainWindowViewModel()
        {
            Document = new CsfDoc();
            MenuListInit();
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void MenuListInit()
        {
            var importList = new List<MenuItem>();
            var exportList = new List<MenuItem>();

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
            foreach (JsonObject item in config as JsonArray)
            {
                var MenuItem = new MenuItem();

                var info = ReflectionInit(item);
                if (info is null)
                {
                    Logger.Warn("导入失败:{0}", item);
                    continue;
                }
                MenuItem.Header = info.Name.GetValue(info.Class) as string;
                if (item.TryGetValue("menu_type", out JsonValue menuType))
                {
                    bool isAsync = item.TryGetValue("isAsync", out JsonValue isAsyncJV) ? (bool)isAsyncJV : false;
                    FileDialog fileDialog;
                    if (((string)menuType).Equals("export", StringComparison.OrdinalIgnoreCase))// 是导出方法
                    {
                        fileDialog = new SaveFileDialog();
                        fileDialog.Filter = item.TryGetValue("filter", out JsonValue filter) ? (string)filter : "All File(*.*)|*.*";
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
                    else if (((string)menuType).Equals("import", StringComparison.OrdinalIgnoreCase))// 是导入方法
                    {
                        fileDialog = new OpenFileDialog();
                        fileDialog.Filter = item.TryGetValue("filter", out JsonValue filter) ? (string)filter : "All File(*.*)|*.*";
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
                    else Logger.Info("未知类型:{0}:{1}", menuType, item);
                }
            }

            ImportList = importList;
            ExportList = exportList;
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
        class ReflectionStruct
        {
            public Assembly Assembly;
            public Type Class;
            public PropertyInfo Name;
            public MethodInfo Method;
        }


        public enum PluginType
        {
            REFLECTION, // 反射程序集
            RUN,    // 运行
            CALL,   // 调用DLL
        }
    }
}
