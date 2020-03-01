using CSF.WPF.Core.View;
using CSF.WPF.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.WPF.Core.Data
{
    public class DocumentStruct
    {
        public DocumentStruct(MainWindow window)
        {
            Document = new CsfDoc();
        }

        public CsfDoc Document { get; set; }
        public CsfDocViewModel DocViewModel => Document.DataContext as CsfDocViewModel;
        public string Header => DocViewModel.FilePath is null ? "导入的文件" : DocViewModel.FilePath.Split('\\')[^1];
    }
}
