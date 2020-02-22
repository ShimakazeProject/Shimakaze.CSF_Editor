using CSF.Logmgr;
using CSF.WPF.Core.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Json;
using System.Text;
using System.Windows.Controls;

namespace CSF.WPF.Core.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private CsfDoc document;
        private List<MenuItem> importList;

        public MainWindowViewModel()
        {
            Document = new CsfDoc();
            try
            {
                ImportListInit();
            }
            catch (FileNotFoundException ex)
            {
                Logger.Warn("未找到文件");
            }
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void ImportListInit()
        {
            var list = new List<MenuItem>();
            using var fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\MenuItem.json", FileMode.Open, FileAccess.Read, FileShare.Read);
            var config = JsonValue.Load(fs)["Import"] as JsonArray;
            foreach (var item in config)
            {
                switch ((PluginType)Enum.Parse(typeof(PluginType), ((string)item["type"]).ToUpper(new System.Globalization.CultureInfo("en-US"))))
                {
                    case PluginType.REFLECTION:
                        //System.Reflection.
                        break;
                    case PluginType.RUN:
                        break;
                    case PluginType.CALL:
                        break;
                    default:
                        break;
                }
            }
            ImportList = list;
        }
        public enum PluginType
        {
            REFLECTION, // 反射程序集
            RUN,    // 运行
            CALL,   // 调用DLL
        }
    }
}
