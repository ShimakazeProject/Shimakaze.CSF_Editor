using CSF.Model.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.WPF.Framework.ViewModel
{
    public class DocumentVM : INotifyPropertyChanged
    {
        private Model.Type[] types;
        private Model.Type selectedType;

        public DocumentVM()
        {
            Types = new Model.Type[0];

        }
        //public DocumentVM(string path)
        //{
        //    Types = new Model.Type[0];
        //    OpenCsf(path);
        //}

        public Model.Type[] Types
        {
            get => types; set
            {
                types = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
            }
        }
        public Model.Type SelectedType
        {
            get => selectedType; set
            {
                selectedType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public async Task OpenCsf(string path)
        {
            await Types.FromCSFAsync(path);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
        }
    }
}
