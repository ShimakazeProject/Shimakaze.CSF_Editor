using RA2CsfEditor.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RA2CsfEditor.ValueConverters
{
    class Int2Bool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            value.GetType() == typeof(int)? (value as int? ?? 0) == 0 ? true: false: false;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    class Obj2Label : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            value?.GetType() == typeof(LabelStruct) ? (value as LabelStruct)?.Labels : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    class ValueCountNotOne : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            (value as Model.Value[]).Length != 1 ? true : false;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    class Value2String : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            (value as Model.Value[])[0].ValueString;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            return new Model.Value[1] { value as string };
        }
    }
    class GetStrLbl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value as Label)?.ValueTag;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    class GetStrNum : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value as Label)?.StringNum.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
