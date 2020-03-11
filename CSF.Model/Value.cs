using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSF.Model
{
    /// <summary>
    /// CSF Value struct
    /// </summary>
    public sealed class Value : INotifyPropertyChanged
    {
        #region Public Fields
        public const string STRING = " RTS";
        public const string WSTRING = "WRTS";
        private string extraString;
        private string valueString;
        private int valueLength;
        private string valueFlag;
        private int extraLength;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Public Fields

        #region Public Constructors
        public Value(string value, string evalue = null)
        {
            ValueString = value;
            ValueLength = value.Length;
            ExtraString = evalue;
            ExtraLength = evalue is null ? 0 : evalue.Length;
            ValueFlag = evalue is null ? STRING : WSTRING;
        }
        public Value(string tag, int vlenght, string value, int elenght = 0, string evalue = null)
        {
            ValueFlag = tag;
            ValueLength = vlenght;
            ExtraLength = elenght;
            ValueString = value;
            ExtraString = evalue;
        }
        #endregion Public Constructors
        #region User Properties
        public bool IsWString => this.ValueFlag == WSTRING;
        public bool IsString => this.ValueFlag == STRING;
        #endregion
        #region Public Properties
        /// <summary>
        /// Extra string length <para/>
        /// If this value flag is WSTRING<para/>
        /// Else return 0
        /// </summary>
        public int ExtraLength
        {
            get => extraLength; set
            {
                extraLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtraLength)));
            }
        }
        /// <summary>
        /// Extra string content<para/><c>
        /// If this value flag is WSTRING<para/>
        /// Else return Null</c>
        /// </summary>
        public string ExtraString
        {
            get => extraString; set
            {
                extraString = value;
                ValueFlag = value is null ? STRING : WSTRING;
                ExtraLength = value is null ? 0 : value.Length;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtraLength)));
            }
        }
        /// <summary>
        /// Value Flag have two content<para/>
        /// STRING(Str:Normal string) or WSTRING(StrW:Width string)
        /// </summary>
        public string ValueFlag
        {
            get => valueFlag; set
            {
                valueFlag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueFlag)));
            }
        }
        /// <summary>
        /// Value string length
        /// </summary>
        public int ValueLength
        {
            get => valueLength; set
            {
                valueLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueLength)));
            }
        }
        /// <summary>
        /// Value string content
        /// </summary>
        public string ValueString
        {
            get => valueString; set
            {
                valueString = value;
                ValueLength = value.Length;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueString)));
            }
        }
        #endregion Public Properties

        #region Public Methods
        public static implicit operator (string, string)(Value value) => (value.ValueString, value.ExtraString);

        /// <summary>
        /// Flag Is STRW ? true : false
        /// </summary>
        public static implicit operator bool(Value value) => value.ValueFlag == WSTRING;

        /// <summary>
        /// Get ValueString content
        /// </summary>
        public static implicit operator string(Value value) => value.ValueString;
        public static implicit operator Value(string value) => new Value(value);
        public static implicit operator Value((string, string) value) => new Value(value.Item1, value.Item2);

        public static bool operator !=(Value left, Value right) => !left.Equals(right);
        public static bool operator ==(Value left, Value right) => left.Equals(right);

        public override bool Equals(object obj) =>
            obj != null &&
            obj is Value &&
            ReferenceEquals(this, obj) &&
            ValueLength == (obj as Value).ValueLength &&
            ValueString.Equals(obj as Value)
                ? this && (obj as Value)
                    ? ExtraLength == (obj as Value).ExtraLength && ExtraString.Equals((obj as Value).ExtraString)
                    : true
                : false;
        public override int GetHashCode() => (ValueString, ExtraString).GetHashCode();
        #endregion Public Methods
    }
}
