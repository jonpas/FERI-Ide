using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ide.Converters
{
    public class IsNotProjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            TreeViewItem treeItemValue = (TreeViewItem)value;
            bool booleanValue = true;
            if ((string)treeItemValue.Tag == "Project")
                booleanValue = false;
            return booleanValue;

            //TextBlock selectedItemText = (TextBlock)selectedItemHolder.Children[1];
            //string stringValue = selectedItemText.Text;

            //return !stringValue.StartsWith("Project '");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
