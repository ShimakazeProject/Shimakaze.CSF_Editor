using System;
using System.Globalization;
using System.Windows.Data;

namespace CSF.WPF.Framework.Converter
{
    [ValueConversion(typeof(Model.Value), typeof(string))]
    public class ModelValueToValueStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType() == typeof(Model.Value))
            {
                return (value as Model.Value).ValueString;
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType() == typeof(string))
            {
                return new Model.Value(value as string, parameter as string);
            }
            else return null;
        }
    }
}
