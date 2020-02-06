using System;
using System.Globalization;
using System.Windows.Data;

namespace CSF.WPF.Framework.Converter
{
    [ValueConversion(typeof(Model.Value), typeof(string))]
    public class ModelValueToExtraStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType() == typeof(Model.Value))
            {
                return (value as Model.Value).ExtraString;
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType() == typeof(string))
            {
                return new Model.Value(parameter as string, value as string);
            }
            else return null;
        }
    }
}
