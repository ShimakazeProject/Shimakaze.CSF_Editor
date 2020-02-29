using CSF.WPF.Core.View;
using CSF.WPF.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.WPF.Core.Data
{
    public class DocumentStruct
    {
        public DocumentStruct()
        {
            Document = new CsfDoc();
        }

        public CsfDoc Document { get; set; }
        public CsfDocViewModel DocViewModel => Document.DataContext as CsfDocViewModel;
        public string Header => DocViewModel.FilePath?.Split('\\')[^1];
    }
}
