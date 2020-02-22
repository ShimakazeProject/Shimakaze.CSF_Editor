using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Model
{
    /// <summary>
    /// CSF Value struct
    /// </summary>
    public class Value
    {
        #region Field
        private string extraString;
        private string valueString;
        #endregion

        #region Construction
        public Value(string value, string evalue = null)
        {
            ValueFlag = " RTS";
            Visibility = true;
            ValueString = value;
            ExtraString = evalue;
        }
        public Value(string tag, int vlenght, string value, int elenght = 0, string evalue = null)
        {
            Visibility = true;
            ValueFlag = tag;
            ValueLength = vlenght;
            ExtraLength = elenght;
            ValueString = value;
            ExtraString = evalue;
        }
        #endregion

        #region Property
        /// <summary>
        /// Value Flag have two content<para/>
        /// " RTS"(Str:Normal string) or "WRTS"(StrW:Width string)
        /// </summary>
        public string ValueFlag { get; private set; }
        /// <summary>
        /// Value string length
        /// </summary>
        public int ValueLength { get; private set; }
        /// <summary>
        /// Value string content
        /// </summary>
        public string ValueString
        {
            get => valueString; set
            {
                ValueLength = value.Length;
                valueString = value;
            }
        }
        /// <summary>
        /// Extra string length <para/>
        /// If this value flag is "WRTS"<para/>
        /// Else return 0
        /// </summary>
        public int ExtraLength { get; private set; }
        /// <summary>
        /// Extra string content<para/><c>
        /// If this value flag is "WRTS"<para/>
        /// Else return Null</c>
        /// </summary>
        public string ExtraString
        {
            get => extraString; set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    ValueFlag = "WRTS";
                    extraString = value;
                    ExtraLength = value.Length;
                }
                else
                {
                    ValueFlag = " RTS";
                    extraString = null;
                    ExtraLength = 0;
                }

            }
        }
        /// <summary>
        /// (Not CSF Property) Show in Editor
        /// </summary>
        public bool Visibility { get; set; }
        #endregion

        #region Convert
        /// <summary>
        /// Get ValueString content
        /// </summary>
        public static implicit operator string(Value value) => value.ValueString;
        /// <summary>
        /// Flag Is STRW ? true : false
        /// </summary>
        public static implicit operator bool(Value value) => value.ValueFlag == "WRTS";
        public static implicit operator Value(string value) => new Value(value);
        #endregion

        #region Operator
        public static bool operator ==(Value left, Value right) => left.Equals(right);
        public static bool operator !=(Value left, Value right) => !(left == right);
        #endregion

        #region Override Method
        public override bool Equals(object obj) =>
            obj != null &&
            obj.GetType() == typeof(Value) &&
            ReferenceEquals(this, obj) &&
            ValueLength == (obj as Value).ValueLength &&
            valueString.Equals(obj as Value)
                ? this && (obj as Value)
                    ? ExtraLength == (obj as Value).ExtraLength && ExtraString.Equals((obj as Value).ExtraString)
                    : true
                : false;
        public override int GetHashCode() => (valueString, extraString).GetHashCode();
        #endregion

    }
}
