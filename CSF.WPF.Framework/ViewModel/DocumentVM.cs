using CSF.Model.Extension;
using CSF.WPF.Framework.CommandBase;
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
        private Controls.Document document;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties Fields
        private Model.Type[] types;
        private Model.Type selectedType;
        private Model.Label selectedLabel;
        private string typesNum;
        private string labelsNum;
        private string stringsNum;
        private string selectedTypeLabelsNum;
        private string selectedTypeStringsNum;
        private string selectedLabelStringsNum;
        #endregion

        public DocumentVM(Controls.Document document)
        {
            this.document = document;
            Types = Array.Empty<Model.Type>();

        }

        #region Binding Propertys
        public Model.Type[] Types
        {
            get => types; set
            {
                bool notNull = value != null;
                types = value;
                TypesNum = value?.Length.ToString();
                LabelsNum = notNull ? (from type in value select type.Labels.Length).Sum().ToString() : null;
                StringsNum = notNull ? (from type in value select (from label in type.Labels select label.StringCount).Sum()).Sum().ToString() : null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
            }
        }
        public Model.Type SelectedType
        {
            get => selectedType; set
            {
                selectedType = value;
                SelectedTypeLabelsNum = value?.Labels.Length.ToString();
                SelectedTypeStringsNum = value != null ? (from label in value.Labels select label.StringCount)?.Sum().ToString() : null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
            }
        }
        public Model.Label SelectedLabel
        {
            get => selectedLabel; set
            {
                selectedLabel = value;
                SelectedLabelStringNum = value?.StringCount.ToString();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabel)));
            }
        }
        public string TypesNum
        {
            get => typesNum; set
            {
                typesNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypesNum)));
            }
        }
        public string LabelsNum
        {
            get => labelsNum; set
            {
                labelsNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelsNum)));
            }
        }
        public string StringsNum
        {
            get => stringsNum; set
            {
                stringsNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringsNum)));
            }
        }
        public string SelectedTypeLabelsNum
        {
            get => selectedTypeLabelsNum; set
            {
                selectedTypeLabelsNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTypeLabelsNum)));
            }
        }
        public string SelectedTypeStringsNum
        {
            get => selectedTypeStringsNum; set
            {
                selectedTypeStringsNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTypeStringsNum)));
            }
        }
        public string SelectedLabelStringNum
        {
            get => selectedLabelStringsNum; set
            {
                selectedLabelStringsNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabelStringNum)));
            }
        }
        #endregion

        public RelayCommand Refresh => new RelayCommand(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabel)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypesNum)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelsNum)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringsNum)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTypeLabelsNum)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTypeStringsNum)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabelStringNum)));
        });


        public async void OpenCsf(string path)
        {
            Types = await Import.FromCSF(path);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
        }

    }
}
