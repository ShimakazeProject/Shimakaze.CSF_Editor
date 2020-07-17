using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Shimakaze.ToolKit.CSF.GUI.Converters
{
    [ValueConversion(typeof(System.Windows.Media.Color), typeof(string))]
    public class StringColorConverter : IValueConverter
    {
        public static StringColorConverter Instance { get; } = new StringColorConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => "#"
            + ((System.Windows.Media.Color)value).R.ToString("x")
            + ((System.Windows.Media.Color)value).G.ToString("x")
            + ((System.Windows.Media.Color)value).B.ToString("x");
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value as string);
    }
}
