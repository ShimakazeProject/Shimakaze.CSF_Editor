using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.WPF.Framework.ViewModel
{
    public class DocumentVM: INotifyPropertyChanged
    {
        public Model.Type[] Types { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
