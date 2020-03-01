using CSF.Logmgr;
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
    public class DocumentsViewModel : INotifyPropertyChanged
    {
        private DocumentStruct[] documents = Array.Empty< DocumentStruct>();
        private DocumentStruct selectDocument;
        public DocumentStruct[] Documents
        {
            get => documents; set
            {
                documents = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Documents)));
            }
        }
        public DocumentStruct SelectDocument
        {
            get => selectDocument; set
            {
                selectDocument = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectDocument)));
            }
        }

        public MainWindow Window { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "C&C Strings File(*.CSF)|*.csf"
            };
            if (ofd.ShowDialog() ?? false)
            {
                var list = documents.ToList();
                var doc = new DocumentStruct(Window);
                doc.DocViewModel.Open(ofd.FileName);
                list.Add(doc);
                Documents = list.ToArray();
                SelectDocument = doc;
            }
        }

        public Command.RelayCommand OpenFileCommand => new Command.RelayCommand(OpenFile);
        public Command.RelayCommand MergeFileCommand => new Command.RelayCommand(MergeFile);
        public Command.RelayCommand SaveFileCommand => new Command.RelayCommand(SaveFile);
        public Command.RelayCommand SaveAsFileCommand => new Command.RelayCommand(SaveAsFile);
        public Command.RelayCommand CloseFileCommand => new Command.RelayCommand(CloseFile);
        public Command.RelayCommand AddLabelCommand => new Command.RelayCommand(AddLabel);
        public Command.RelayCommand RemoveLabelCommand => new Command.RelayCommand(RemoveLabel);
        public Command.RelayCommand ChangeLabelCommand => new Command.RelayCommand(ChangeLabel);

        internal void MergeFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "C&C Strings File(*.CSF)|*.csf"
            };
            if (MessageBox.Show("合并时是否允许覆盖已存在的标签?", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (ofd.ShowDialog() ?? false)
                {
                    var ret = SelectDocument.DocViewModel.Merge(ofd.FileName);
                    MessageBox.Show(string.Format("覆盖{0}个重复标签,添加{1}个标签,共{2}个标签", ret.Item1, ret.Item2, ret.Item1 + ret.Item2), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (ofd.ShowDialog() ?? false)
                {
                    var ret = SelectDocument.DocViewModel.Merge(ofd.FileName, false);
                    MessageBox.Show(string.Format("添加{1}个标签,且发现但未并入{0}个重复标签", ret.Item1, ret.Item2), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        internal void SaveFile()
        {
            if (string.IsNullOrEmpty(SelectDocument.DocViewModel.FilePath))
            {
                SaveAsFile();
            }
            else SelectDocument.DocViewModel.Save();
        }
        internal void SaveAsFile()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "C&C Strings File(*.CSF)|*.csf"
            };
            if (sfd.ShowDialog() ?? false)
            {
                SelectDocument.DocViewModel.SaveAs(sfd.FileName);
            }
        }
        internal void CloseFile()
        {
            var list = documents.ToList();
            list.Remove(SelectDocument);
            Documents = list.ToArray();
        }


        internal void AddLabel()
        {
            SelectDocument.DocViewModel.AddLabel();
        }
        internal void RemoveLabel()
        {
            SelectDocument.DocViewModel.DropLabel();
        }
        internal void ChangeLabel()
        {
            SelectDocument.DocViewModel.EditLabel();
        }
        internal async Task Converter(object ret)
        {
            if (ret is Model.File)
            {
                var typeSet = new Model.TypeSet();
                typeSet.MakeType(ret as Model.File);
                SelectDocument.DocViewModel.TypeList = typeSet;
                Logger.Info("Plugin : DONE.");
            }
            else if (ret is Task<Model.File>)
            {
                var typeSet = new Model.TypeSet();
                typeSet.MakeType(await (ret as Task<Model.File>).ConfigureAwait(true));
                SelectDocument.DocViewModel.TypeList = typeSet;
                Logger.Info("Plugin : DONE.");
            }
            else
            {
                Logger.Warn("Plugin : Unknow Type.");
                //MessageBox.Show("操作未成功", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        internal void Import(Model.File file)
        {
            var list = documents.ToList();
            var doc = new DocumentStruct(Window);
            var typeSet = new Model.TypeSet();
            typeSet.MakeType(file);
            doc.DocViewModel.TypeList = typeSet;
            list.Add(doc);
            Documents = list.ToArray();
            SelectDocument = doc;
            Logger.Info("Plugin : DONE.");
        }
    }
}
