using CSF.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Model
{
    public class Label : ILabel, INotifyPropertyChanged, Interface.IVisibility
    {
        #region Field
        private string labelName;
        private IValue[] values;
        private int nameLength;
        private int stringCount;
        private bool visibility;
        #endregion

        #region Construction
        public Label(string labelTag, int stringCount, int nameLength, string labelName, Value[] values)
        {
            Visibility = true;
            LabelTag = labelTag;
            StringCount = stringCount;
            NameLength = nameLength;
            LabelName = labelName;
            Values = values;
            foreach (var value in values)
                value.PropertyChanged += (o, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
        }
        public Label(string nameLength, Value[] values) : this(" LBL", values.Count(), nameLength.Length, nameLength, values) { }
        public Label(ILabel label)
        {
            Visibility = true;
            LabelTag = label.LabelTag;
            StringCount = label.StringCount;
            NameLength = label.NameLength;
            LabelName = label.LabelName;
            var convert = new Value[0];
            foreach (var ivalue in label.Values)
            {
                var value = new Value(ivalue);
                value.PropertyChanged += (o, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
                convert = convert.Concat(new Value[] { value }).ToArray();
            }
            Values = convert;

        }
        #endregion

        #region Property
        public string LabelTag { get; private set; }
        public int StringCount
        {
            get => stringCount; private set
            {
                stringCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringCount)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
            }
        }
        public int NameLength
        {
            get => nameLength; private set
            {
                nameLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NameLength)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
            }
        }
        public string LabelName
        {
            get => labelName; set
            {
                NameLength = value.Length;
                labelName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelName)));
            }
        }
        public IValue[] Values
        {
            get => values; set
            {
                StringCount = value.Count();
                values = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
            }
        }
        //public int Length => 0x0C + NameLength + (from value in Values select value.Length).Sum();
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

        #region Indexer
        public Value this[int index]
        {
            get => Values.ToArray()[index] as Value;
            set
            {
                var array = Values.ToArray();
                array[index] = value;
                Values = array;
            }
        }
        #endregion
    }
}
