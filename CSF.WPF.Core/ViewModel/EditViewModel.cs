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
        private string labelName;
        private ValueStruct selectedItem;
        private ValueStruct[] values;
        private bool visibility = false;
        private Model.Label label;
        #endregion Private Fields

        #region Public Constructors
        public EditViewModel()
        {
            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName is nameof(Values) || e.PropertyName is nameof(SelectedItem))
                {
                    CanRemove = (SelectedItem is null || values is null) ? false : values.Length > 1 ? true : false;
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
                //System.Text.RegularExpressions.Regex.
                labelName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelName)));
            }
        }


        public Model.Label Label
        {
            get => label; set
            {
                label = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Label)));
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

        public ValueStruct[] Values
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
            var valueList = new List<ValueStruct>(values);
            valueList.Add(new ValueStruct { Value = "New Value", ID = values?.Length is null ? 0 : values.Length });
            Values = valueList.ToArray();
        }
        

        public void Cancel()
        {
            Visibility = false;
            LabelName = null;
            Values = null;
        }

        public void OK()
        {
            var values = new Model.Value[Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                values[i] = this.values[i];
            }
            //label.Changed(new Model.Label(labelName, values));
            //BaseVM.ReCount();
            //BaseVM.TypeList.MakeType();
            Cancel();
        }
        public static void StaticMethod()
        {
        }

        public virtual void VirtualMethod()
        {
        }

        public void Remove()
        {
            var valueList = new List<ValueStruct>(values);
            valueList.Remove(SelectedItem);
            Values = valueList.ToArray();
        }

        public void SetLabel(Model.Label value)
        {
            Label = value;
            if (value is null) return;
            LabelName = value.LabelName;
            List<ValueStruct> values = new List<ValueStruct>(value.LabelValues.Length);
            foreach (var v in value.LabelValues)
            {
                values.Add(new ValueStruct { Extra = v.ExtraString, Value = v.ValueString, ID = values?.Count is null ? 0 : values.Count });
            }
            Values = values.ToArray();
            SelectedItem = Values[0];
            Visibility = true;
        }
        #endregion Public Methods
    }
}