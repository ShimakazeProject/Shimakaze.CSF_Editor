using CSF.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSF.WPF.Core.ViewModel
{
    public class CsfDocViewModel : INotifyPropertyChanged
    {
        private string filePath;
        private Label data;
        private Model.Type dataList;
        private TypeSet typeList;

        public CsfDocViewModel()
        {
            TypeList = new TypeSet();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Binding Property
        public Label Data
        {
            get => data; set
            {
                data = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
            }
        }
        public Model.Type DataList
        {
            get => dataList; set
            {
                dataList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataList)));
            }
        }
        public TypeSet TypeList
        {
            get => typeList; set
            {
                typeList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypeList)));
            }
        }
        public string FilePath
        {
            get => filePath; set
            {
                filePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
            }
        }
        #endregion

        public void Open(string path)
        {
            FilePath = path;
            var typelist = new TypeSet();
            typelist.LoadFromFile(path);
            TypeList = typelist;
        }
        public void SaveAs(string path)
        {
            FilePath = path;
        }
    }
}
