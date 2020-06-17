using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA2CsfEditor.Model
{
    public class Label : INotifyPropertyChanged, ILabel
    {
        private string extraValue;
        private string labelString;
        private int? extraValueLength;
        private Value[] values;
        private bool visibility;

        public string LabelTag { get; private set; }
        public int StringNum { get; private set; }
        public int LabelLength { get; private set; }
        public string LabelString
        {
            get => labelString; set
            {
                labelString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelString)));
            }
        }
        public string ValueTag { get; private set; }
        public Value[] Values
        {
            get => values; set
            {
                values = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
            }
        }
        public int? ExtraValueLength
        {
            get => extraValueLength; private set
            {
                extraValueLength = (value ?? 0) > 0 ? value : null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtraValueLength)));
            }
        }
        public string ExtraValue
        {
            get => extraValue; set
            {
                ValueTag = "WRTS";
                if (!string.IsNullOrWhiteSpace(value))
                {
                    extraValue = value;
                    ExtraValueLength = value?.Length;
                }
                else
                {
                    ValueTag = " RTS";
                    extraValue = null;
                    ExtraValueLength = null;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtraValue)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueTag)));
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


        public Label(string LabelTag, int StringNum, int LabelLength, string LabelString, string ValueTag, IEnumerable<Value> Values, int? ExtraValueLength = null, string ExtraValue = null)
        {
            Visibility = true;
            this.LabelTag = LabelTag;
            this.StringNum = StringNum;
            this.LabelLength = LabelLength;
            this.LabelString = LabelString;
            this.ValueTag = ValueTag;
            foreach (var item in Values)
            {
                item.PropertyChanged += (o, e) =>
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Values)));
            }
            this.Values = Values.ToArray();

            this.ExtraValueLength = ExtraValueLength;
            this.ExtraValue = ExtraValue;
        }
        public Label(string LabelString, IEnumerable<Value> Values, string ExtraValue = null)
            : this(" LBL", Values.Count(), LabelString.Length, LabelString, !string.IsNullOrEmpty(ExtraValue) ? "WRTS" : " RTS", Values, ExtraValue?.Length, ExtraValue) { }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool LBLequal(Label a)
        {
            return a.LabelString == LabelString;
        }
        public static bool LBLequal(Label a, Label b)
        {
            return a.LabelString == b.LabelString;
        }
    }
}
