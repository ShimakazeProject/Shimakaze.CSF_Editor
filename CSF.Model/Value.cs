using CSF.Model.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Model
{
    public class Value : Core.IValue, INotifyPropertyChanged, IVisibility
    {
        #region Field
        private bool visibility;
        private string extraString;
        private int? extraLength;
        private string valueString;
        private int valueLength;
        private string valueTag;
        #endregion

        #region Construction
        public Value(string value,string evalue=null)
        {
            ValueTag = " RTS";
            ValueString = value;
            Visibility = true;
            ExtraString = evalue;
        }
        public Value(Core.IValue value)
        {
            ValueTag = value.ValueTag;
            ValueString = value.ValueString;
            Visibility = true;
            ExtraString = value.ExtraString;
        }
        #endregion

        #region Property
        public string ValueTag
        {
            get => valueTag; private set
            {
                valueTag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueTag)));
            }
        }
        public int ValueLength
        {
            get => valueLength; private set
            {
                valueLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueLength)));
            }
        }
        public string ValueString
        {
            get => valueString; set
            {
                ValueLength = value.Length;
                valueString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueString)));
            }
        }
        public int? ExtraLength
        {
            get => extraLength; private set
            {
                extraLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtraLength)));
            }
        }
        public string ExtraString
        {
            get => extraString; set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    ValueTag = "WRTS";
                    extraString = value;
                    ExtraLength = value?.Length;
                }
                else
                {
                    ValueTag = " RTS";
                    extraString = null;
                    ExtraLength = null;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtraString)));
            }
        }
        //public int Length => ValueLength * 2 + 0x0C + (ExtraLength ?? 0);
        public bool Visibility
        {
            get => visibility; set
            {
                visibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visibility)));
            }
        }
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Operator
        public static implicit operator string(Value value) => value.ValueString;
        public static implicit operator Value(string value) => new Value(value);
        #endregion
    }
}
