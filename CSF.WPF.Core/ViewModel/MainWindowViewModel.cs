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

        public MainWindowViewModel()
        {
            Documents = new DocumentsViewModel();
            Documents.PropertyChanged += (o, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Documents)));
            Data.Plugin.PluginManager.Documents = Documents;
            Editor = new Editor();
        }

        public bool SearchModeTitle { get; set; } = true;
        public bool SearchModeValue { get; set; }
        public bool SearchModeExtra { get; set; }
        public bool SearchModeFull { get; set; }
        public bool SearchModeRegex { get; set; }
        public bool SearchIgnoreCase { get; set; } = true;

        public List<MenuItem> ImportList => Data.Plugin.PluginManager.ImportList;
        public List<MenuItem> ExportList => Data.Plugin.PluginManager.ExportList;
        public List<MenuItem> ConverterList => Data.Plugin.PluginManager.ConverterList;
        public List<MenuItem> SingleConverterList => Data.Plugin.PluginManager.SingleConverterList;
        public List<MenuItem> PluginList => Data.Plugin.PluginManager.PluginList;

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

        public Command.RelayCommand OpenFileCommand => new Command.RelayCommand(Documents.OpenFile);
        public Command.RelayCommand MergeFileCommand => new Command.RelayCommand(Documents.MergeFile);
        public Command.RelayCommand SaveFileCommand => new Command.RelayCommand(Documents.SaveFile);
        public Command.RelayCommand SaveAsFileCommand => new Command.RelayCommand(Documents.SaveAsFile);

        public Command.RelayCommand CloseFileCommand => new Command.RelayCommand(Documents.CloseFile);
        public Command.RelayCommand<Window> ExitCommand => new Command.RelayCommand<Window>(Exit);

        public Command.RelayCommand AddLabelCommand => new Command.RelayCommand(Documents.AddLabel);
        public Command.RelayCommand RemoveLabelCommand => new Command.RelayCommand(Documents.RemoveLabel);
        public Command.RelayCommand ChangeLabelCommand => new Command.RelayCommand(Documents.ChangeLabel);
        public Command.RelayCommand<string> SearchCommand => new Command.RelayCommand<string>(Search);
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
        private void Exit(Window window) => window.Close();
        private async void About()
        {
            await dialogCoordinator.ShowMessageAsync(this, "关于此编辑器", string.Format("内部预览版 build:10654{0}Copyright © 2019 - 2020  舰队的偶像-岛风酱!", Environment.NewLine));
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
