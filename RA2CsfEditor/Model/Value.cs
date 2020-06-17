using System.ComponentModel;
using System.Linq;

namespace RA2CsfEditor.Model
{
    public class Value : INotifyPropertyChanged
    {
        private string valueString;

        public event PropertyChangedEventHandler PropertyChanged;
        public int ValueLength { get; private set; }
        public string ValueString
        {
            get => valueString; set
            {
                valueString = value;
                ValueLength = value.Length;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueString)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueLength)));
            }
        }
        public Value(int length, string value)
        {
            ValueString = value;
            ValueLength = length;
        }
        public Value(string value)
        {
            ValueString = value;
        }

        public static implicit operator string(Value value)
        {
            return value.ValueString;
        }
        public static implicit operator Value(string value)
        {
            return new Value(value);
        }


        public static System.Collections.Generic.IEnumerable<Value> ConvertArray(System.Collections.Generic.IEnumerable<string> strs) => from str in strs select (Value)str;
        public static System.Collections.Generic.IEnumerable<string> ConvertArray(System.Collections.Generic.IEnumerable<Value> strs) => from str in strs select (string)str;
    }
}
