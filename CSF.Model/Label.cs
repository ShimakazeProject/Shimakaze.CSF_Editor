using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CSF.Model
{
    public class Label:IEnumerable
    {
        #region Field
        private string labelName;
        private Value[] values;
        #endregion

        #region Construction
        public Label(string labelTag, int stringCount, int nameLength, string labelName,Value[] values)
        {
            Visibility = true;
            LabelFlag = labelTag;
            LabelStrCount = stringCount;
            LabelNameLength = nameLength;
            LabelName = labelName;
            LabelValues = values;
        }
        public Label(string nameLength,Value[] values) : this(" LBL", 1, nameLength.Length, nameLength, values) { }
        #endregion

        #region Property
        public string LabelFlag { get; private set; }
        public int LabelStrCount { get; private set; }
        public int LabelNameLength { get; private set; }
        public string LabelName
        {
            get => labelName; set
            {
                LabelNameLength = value.Length;
                labelName = value;                
            }
        }
        public Value[] LabelValues
        {
            get => values; set
            {
                LabelStrCount = value.Length;
                values = value;
            }
        }
        public bool Visibility { get; set; }
        #endregion

        #region Operator
        public static implicit operator KeyValuePair<string,Value[]>(Label label) => new KeyValuePair<string, Value[]>(label.labelName, label.LabelValues);
        public static implicit operator (string, Value[])(Label label) => (label.labelName, label.LabelValues);

        public static implicit operator Label(KeyValuePair<string, Value[]> keyValue) => new Label(keyValue.Key, keyValue.Value);
        public static implicit operator Label((string, Value[]) kv) => new Label(kv.Item1, kv.Item2);
        #endregion

        #region Indexer
        public Value this[int index]
        {
            get => LabelValues[index];
            set => LabelValues[index] = value;
        }
        #endregion

        public IEnumerator GetEnumerator() => LabelValues.GetEnumerator();
    }
}
