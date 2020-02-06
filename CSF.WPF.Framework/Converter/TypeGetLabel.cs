using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CSF.WPF.Framework.Converter
{
    [ValueConversion(typeof(Model.Type), typeof(Dictionary<Model.Label, List<Model.Value>>))]
    public class Type2List4Label : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType() == typeof(Model.Type))
            {
                return (value as Model.Type).Labels;
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
