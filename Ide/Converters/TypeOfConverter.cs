using System;
using System.Globalization;
using System.Windows.Data;

namespace Ide.Converters
{
    [ValueConversion(typeof(object), typeof(Type))]
    class TypeOfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
