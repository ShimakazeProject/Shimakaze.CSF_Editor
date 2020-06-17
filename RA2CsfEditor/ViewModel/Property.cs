using RA2CsfEditor.Command;
using RA2CsfEditor.Model;
using System;
using System.ComponentModel;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace RA2CsfEditor.ViewModel
{
    public partial class MainVM : INotifyPropertyChanged
    {
        #region 成员变量
        private CSF ThisCSF;// CSF 文件
        private string _path;// 记忆的上次路径
        private string label;
        private string strings;
        private int maxValue;
        private int thisValue;
        private LabelStruct[] labels;
        private LabelStruct selectedItem;
        private bool searchModeFull;
        private bool searchModeRegex;
        private bool isIndeterminate;
        private bool searchModeTitle;
        private bool searchModeValue;


        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
        #region 用户属性
        public string StringsNumber
        {
            get => strings;
            private set
            {
                strings = value;
                App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, $"[MainVM] Strings={value}");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringsNumber)));
            }
        }
        public string LabelsNumber
        {
            get => label;
            private set
            {
                label = value;
                App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, $"[MainVM] Label={value}");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelsNumber)));
            }
        }        
        public LabelStruct[] Labels // 左侧列表ItemsSource
        {
            get => labels;
            private set
            {
                labels = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Labels)));
            }
        }
        public LabelStruct SelectedLabel // 右侧列表ItemsSource 左侧列表SelectedItem
        {
            get => selectedItem; set
            {
                selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabel)));
            }
        }
        public int Status
        {
            get => thisValue;
            private set
            {
                thisValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
            }
        }
        public int MaxStatus
        {
            get => maxValue;
            private set
            {
                maxValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxStatus)));
            }
        }
        public bool SearchModeTitle // 搜索标签SearchLabel_Mode
        {
            get => searchModeTitle; set
            {
                searchModeTitle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchModeTitle)));
            }
        }
        public bool SearchModeValue
        {
            get => searchModeValue; set
            {
                searchModeValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchModeValue)));
            }
        }
        public bool SearchModeFull
        {
            get => searchModeFull; set
            {
                searchModeFull = value;
                searchModeRegex = value ? false : searchModeRegex;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchModeFull)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchModeRegex)));
            }
        }
        public bool SearchModeRegex
        {
            get => searchModeRegex; set
            {
                searchModeRegex = value;
                searchModeFull = value ? false : searchModeFull;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchModeFull)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchModeRegex)));
            }
        }
        public bool IsIndeterminate
        {
            get => isIndeterminate; set
            {
                isIndeterminate = value;
                if (value) Status = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIndeterminate)));
            }
        }
        // Command
        public RelayCommand<Window> ExitCommand => new RelayCommand<Window>(Exit);
        public RelayCommand<string> SearchCommand => new RelayCommand<string>(Search);
        public RelayCommand OpenCommand => new RelayCommand(Open);
        public RelayCommand AppendCommand => new RelayCommand(Append);
        public RelayCommand SaveCommand => new RelayCommand(Save);
        public RelayCommand SaveAsCommand => new RelayCommand(SaveAs);
        public RelayCommand CloseCommand => new RelayCommand(Close);
        public RelayCommand ExportJsonCommand => new RelayCommand(ExportJson);
        public RelayCommand ImportJsonCommand => new RelayCommand(ImportJson);
        public RelayCommand ExportXmlCommand => new RelayCommand(ExportXml);
        public RelayCommand ImportXmlCommand => new RelayCommand(ImportXml);
        public RelayCommand ExportTxtCommand => new RelayCommand(ExportTxt);
        public RelayCommand<Label> EditLabelCommand => new RelayCommand<Label>(EditLabel);
        public RelayCommand ReloadListCommand => new RelayCommand(ReloadList);

        #endregion
    }
}
