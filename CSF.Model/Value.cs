using CSF.Model.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Model
{
    public class Value : INotifyPropertyChanged, IVisibility
    {
        #region Field
        private bool visibility;
        private string extraString;
        private int? extraLenght;
        private string valueString;
        private int valueLenght;
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
        public Value(string tag,int vlenght,string value,int? elenght=null, string evalue = null)
        {
            Visibility = true;
            ValueTag = tag;
            ValueLenght = vlenght;
            ValueString = value;
            ExtraLenght = elenght;
            ExtraString = evalue;
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
        public int ValueLenght
        {
            get => valueLenght; private set
            {
                valueLenght = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueLenght)));
            }
        }
        public string ValueString
        {
            get => valueString; set
            {
                ValueLenght = value.Length;
                valueString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueString)));
            }
        }
        public int? ExtraLenght
        {
            get => extraLenght; private set
            {
                extraLenght = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtraLenght)));
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
                    ExtraLenght = value?.Length;
                }
                else
                {
                    ValueTag = " RTS";
                    extraString = null;
                    ExtraLenght = null;
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
