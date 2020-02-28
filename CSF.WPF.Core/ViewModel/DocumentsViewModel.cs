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
        private List<DocumentStruct> documents = new List<DocumentStruct>();
        private DocumentStruct selectDocument;
        public List<DocumentStruct> Documents
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "C&C Strings File(*.CSF)|*.csf"
            };
            if (ofd.ShowDialog() ?? false)
            {
                var list = documents;
                var doc = new DocumentStruct();
                doc.DocViewModel.Open(ofd.FileName);
                list.Add(doc);
                Documents = list;
            }
        }
    }
}
