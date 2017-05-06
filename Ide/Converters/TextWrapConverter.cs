using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ide.Converters
{
    [ValueConversion(typeof(TextWrapping), typeof(bool))]
    public class TextWrapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TextWrapping wrap = (TextWrapping)value;
            bool booleanWrap = false;
            if (wrap == TextWrapping.Wrap)
                booleanWrap = true;
            return booleanWrap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanWrap = (bool)value;
            TextWrapping wrap = TextWrapping.NoWrap;
            if (booleanWrap)
                wrap = TextWrapping.Wrap;
            return wrap;
        }
    }
}
