using CSF.WPF.Core.View;
using CSF.WPF.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.WPF.Core.Data
{
    public class DocumentStruct
    {
        private CsfDoc document;

        public DocumentStruct()
        {
            Document = new CsfDoc();
        }

        public CsfDoc Document
        {
            get => document; set
            {
                document = value;
                DocViewModel = Document.DataContext as CsfDocViewModel;
            }
        }
        public CsfDocViewModel DocViewModel { get; private set; } 
        public string Header => DocViewModel.FilePath is null ? "导入的文件" : DocViewModel.FilePath.Split('\\')[^1];
    }
}
