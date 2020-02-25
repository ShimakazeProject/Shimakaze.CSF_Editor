using CSF.WPF.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSF.WPF.Core.ViewModel
{
    public class EditViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        private bool canRemove;
        private Model.Label label;
        private string labelName;
        private ValueStruct selectedItem;
        private List<ValueStruct> values;
        private bool visibility = false;
        private bool needRefresh = false;
        #endregion Private Fields

        #region Public Constructors
        public EditViewModel()
        {
            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName is nameof(Values) || e.PropertyName is nameof(SelectedItem))
                {
                    CanRemove = (SelectedItem is null || values is null) ? false : values.Count > 1 ? true : false;
                }
            };
        }
        #endregion Public Constructors

        #region Public Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Public Events

        #region Public Properties

        public bool CanRemove
        {
            get => canRemove; set
            {
                canRemove = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRemove)));
            }
        }

        public string LabelName
        {
            get => labelName; set
            {
                labelName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelName)));
            }
        }


        public ValueStruct SelectedItem
        {
            get => selectedItem; set
            {
                selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }

        public List<ValueStruct> Values
        {
            get => values; set
            {
                values = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
            }
        }

        public bool Visibility
        {
            get => visibility; set
            {
                visibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visibility)));
            }
        }
        public CsfDocViewModel BaseVM { get; set; }
        #endregion Public Properties

        #region Commands
        public Command.RelayCommand CancelCommand => new Command.RelayCommand(Cancel);
        public Command.RelayCommand OKCommand => new Command.RelayCommand(OK);
        public Command.RelayCommand RemoveCommand => new Command.RelayCommand(Remove);
        public Command.RelayCommand AddCommand => new Command.RelayCommand(Add);
        #endregion

        #region Public Methods
        public void Add()
        {
            Values.Add(new ValueStruct { Value = "New Value", ID = values?.Count is null ? 0 : values.Count });
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
        }

        public void Cancel()
        {
            Visibility = false;
            LabelName = null;
            Values.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
        }

        public void OK()
        {
            var values = new Model.Value[Values.Count];
            for (int i = 0; i < Values.Count; i++)
            {
                values[i] = this.values[i];
            }
            label.Changed(new Model.Label(labelName, values));
            BaseVM.ReCount();
            if (needRefresh)
            {
                BaseVM.TypeList.MakeType();
                needRefresh = false;
            }
            Cancel();
        }

        public void Remove()
        {
            Values.Remove(SelectedItem);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
        }

        public void SetLabel(Model.Label value, bool needRefresh = false)
        {
            this.needRefresh = needRefresh;
            label = value;
            if (value is null) return;
            LabelName = value.LabelName;
            List<ValueStruct> values = new List<ValueStruct>(value.LabelValues.Length);
            foreach (var v in value.LabelValues)
            {
                values.Add(new ValueStruct { Extra = v.ExtraString, Value = v.ValueString, ID = values?.Count is null ? 0 : values.Count });
            }
            Values = values;
            SelectedItem = Values[0];
            Visibility = true;
        }
        #endregion Public Methods
    }
}