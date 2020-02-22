using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CSF.Model
{
    public sealed class Label : IEnumerable
    {
        #region Public Fields
        public const string LABEL = " LBL";
        #endregion Public Fields

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
        public string LabelFlag { get; set; }
        public string LabelName { get; set; }
        public int LabelNameLength { get; set; }
        public int LabelStrCount { get; set; }
        public Value[] LabelValues { get; set; }
        public bool Visibility { get; set; }
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
