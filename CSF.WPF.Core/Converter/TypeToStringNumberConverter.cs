using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CSF.WPF.Core.Converter
{
    [ValueConversion(typeof(Model.Type), typeof(int))]
    public class TypeToStringNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? null : (int?)(from Model.Label label in ((Model.Type)value).Labels select label.LabelStrCount).Sum();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
