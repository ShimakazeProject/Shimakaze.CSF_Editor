using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSF.Model
{
    public sealed class Label : IEnumerable, INotifyPropertyChanged
    {
        public const string LABEL = " LBL";

        private string labelName;
        private Value[] labelValues;
        private int labelStrCount;
        private int labelNameLength;
        private string labelFlag;
        private bool visibility;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Public Constructors
        public Label(string labelTag, int stringCount, int nameLength, string labelName, Value[] values)
        {
            Visibility = true;
            LabelFlag = labelTag;
            LabelStrCount = stringCount;
            LabelNameLength = nameLength;
            LabelName = labelName;
            LabelValues = values;
        }
        public Label(string labelName, Value[] values)
        {

            Visibility = true;
            LabelFlag = LABEL;
            LabelStrCount = values.Length;
            LabelNameLength = labelName.Length;
            LabelName = labelName;
            LabelValues = values;
        }
        #endregion Public Constructors

        #region Public Properties
        public string LabelFlag
        {
            get => labelFlag; set
            {
                labelFlag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelFlag)));
            }
        }
        public string LabelName
        {
            get => labelName; set
            {
                labelName = value;
                LabelNameLength = value.Length;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelName)));
            }
        }
        public int LabelNameLength
        {
            get => labelNameLength; set
            {
                labelNameLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelNameLength)));
            }
        }
        public int LabelStrCount
        {
            get => labelStrCount; set
            {
                labelStrCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelStrCount)));
            }
        }
        public Value[] LabelValues
        {
            get => labelValues; set
            {
                labelValues = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelValues)));
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

        #region Public Indexers
        public Value this[int index]
        {
            get => LabelValues[index];
            set => LabelValues[index] = value;
        }
        #endregion Public Indexers

        #region Public Methods
        public static implicit operator KeyValuePair<string, Value[]>(Label label) => new KeyValuePair<string, Value[]>(label.LabelName, label.LabelValues);
        public static implicit operator Label(KeyValuePair<string, Value[]> keyValue) => new Label(keyValue.Key, keyValue.Value);

        public static implicit operator (string, Value[])(Label label) => (label.LabelName, label.LabelValues);
        public static implicit operator Label((string, Value[]) kv) => new Label(kv.Item1, kv.Item2);
        public void Changed(Label newLabel)
        {
            this.LabelFlag = newLabel.LabelFlag;
            this.LabelStrCount = newLabel.LabelStrCount;
            this.LabelNameLength = newLabel.LabelNameLength;
            this.LabelName = newLabel.LabelName;
            this.LabelValues = newLabel.LabelValues;
        }

        public IEnumerator GetEnumerator() => LabelValues.GetEnumerator();
        #endregion Public Methods
    }
}
