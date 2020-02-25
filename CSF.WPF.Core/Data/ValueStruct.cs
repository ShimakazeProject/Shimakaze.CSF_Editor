namespace CSF.WPF.Core.Data
{
    public class ValueStruct
    {
        public string Value { get; set; }
        public string Extra { get; set; }
        public int ID { get; set; }
        public static implicit operator Model.Value(ValueStruct value)
        {
            if (value is null) return null;
            return value.ToValue();
        }

        public Model.Value ToValue() => new Model.Value(Value, Extra);
    }
}