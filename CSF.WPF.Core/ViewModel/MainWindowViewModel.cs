using CSF.Logmgr;
using CSF.Plugin;
using CSF.WPF.Core.Data;
using CSF.WPF.Core.View;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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

        private readonly IDialogCoordinator dialogCoordinator = DialogCoordinator.Instance;
        private bool showEditor;
        private string searchText;

        public MainWindowViewModel()
        {
            Documents = new DocumentsViewModel();
            Documents.PropertyChanged += (o, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Documents)));
            Data.Plugin.PluginManager.Documents = Documents;
            Editor = new Editor();
        }
        public string Title => string.Format("CSF文件编辑器•改二 *评估版本 Build:{0}*", Program.BUILD);
        public bool SearchModeTitle { get; set; } = true;
        public bool SearchModeValue { get; set; }
        public bool SearchModeExtra { get; set; }
        public bool SearchModeFull { get; set; }
        public bool SearchModeRegex { get; set; }
        public bool SearchIgnoreCase { get; set; } = true;

        public string SearchText
        {
            get => searchText; set
            {
                searchText = value;
                Search(value);
            }
        }

        public DocumentsViewModel Documents { get; set; }
        public Editor Editor { get; set; }
        public bool ShowEditor
        {
            get => showEditor; set
            {
                showEditor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowEditor)));
            }
        }
        public Command.RelayCommand<Window> ExitCommand { get; } = new Command.RelayCommand<Window>(Exit);

        public Command.RelayCommand AboutCommand => new Command.RelayCommand(About);




        public event PropertyChangedEventHandler PropertyChanged;

        //private void OpenFile()
        //{
        //    OpenFileDialog ofd = new OpenFileDialog
        //    {
        //        Filter = "C&C Strings File(*.CSF)|*.csf"
        //    };
        //    if (ofd.ShowDialog() ?? false)
        //    {
        //        Documents.SelectDocument.DocViewModel.Open(ofd.FileName);
        //    }
        //}
        //private void MergeFile()
        //{
        //    OpenFileDialog ofd = new OpenFileDialog
        //    {
        //        Filter = "C&C Strings File(*.CSF)|*.csf"
        //    };
        //    if (MessageBox.Show("合并时是否允许覆盖已存在的标签?", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //    {
        //        if (ofd.ShowDialog() ?? false)
        //        {
        //            var ret = Documents.SelectDocument.DocViewModel.Merge(ofd.FileName);
        //            MessageBox.Show(string.Format("覆盖{0}个重复标签,添加{1}个标签,共{2}个标签", ret.Item1, ret.Item2, ret.Item1 + ret.Item2), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //    }
        //    else
        //    {
        //        if (ofd.ShowDialog() ?? false)
        //        {
        //            var ret = Documents.SelectDocument.DocViewModel.Merge(ofd.FileName, false);
        //            MessageBox.Show(string.Format("添加{1}个标签,且发现但未并入{0}个重复标签", ret.Item1, ret.Item2), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //    }
        //}
        //private void SaveFile()
        //{
        //    if (string.IsNullOrEmpty(Documents.SelectDocument.DocViewModel.FilePath))
        //    {
        //        SaveAsFile();
        //    }
        //    else Documents.SelectDocument.DocViewModel.Save();
        //}
        //private void SaveAsFile()
        //{
        //    SaveFileDialog sfd = new SaveFileDialog
        //    {
        //        Filter = "C&C Strings File(*.CSF)|*.csf"
        //    };
        //    if (sfd.ShowDialog() ?? false)
        //    {
        //        Documents.SelectDocument.DocViewModel.SaveAs(sfd.FileName);
        //    }
        //}
        //private void CloseFile()
        //{
        //    Documents.SelectDocument.DocViewModel.TypeList = new Model.TypeSet();
        //}
        //private void AddLabel()
        //{
        //    Documents.SelectDocument.DocViewModel.AddLabel();
        //}
        //private void RemoveLabel()
        //{
        //    Documents.SelectDocument.DocViewModel.DropLabel();
        //}
        //private void ChangeLabel()
        //{
        //    Documents.SelectDocument.DocViewModel.EditLabel();
        //}
        private static void Exit(Window window) => window.Close();
        private async void About()
        {
            await dialogCoordinator.ShowMessageAsync(this, "关于此编辑器",
                string.Format("预发行版 build:{1}{0}Copyright © 2019 - 2020  舰队的偶像-岛风酱!",
                Environment.NewLine, Program.BUILD));
        }
        private void Search(string s)
        {
            Data.SearchMode mode = SearchMode.None;
            if (SearchModeTitle) mode |= SearchMode.Label;
            if (SearchModeValue) mode |= SearchMode.Value;
            if (SearchModeExtra) mode |= SearchMode.Extra;
            if (SearchModeFull) mode |= SearchMode.Full;
            if (SearchModeRegex) mode |= SearchMode.Regex;
            if (SearchIgnoreCase) mode |= SearchMode.IgnoreCase;

            Documents.SelectDocument.DocViewModel.Search(s, mode);
        }

        public void EditLabel(CsfDocViewModel thisVM, Model.Label label)
        {
            (Editor.DataContext as EditViewModel).BaseVM = thisVM;
            (Editor.DataContext as EditViewModel).SetLabel(label);
            ShowEditor = true;
        }
    }
}
