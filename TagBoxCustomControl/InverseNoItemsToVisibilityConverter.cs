using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MultipleItemSelectorCustomControl
{
    public class InverseNoItemsToVisibilityConverter:IValueConverter
    {
        public static InverseNoItemsToVisibilityConverter Instance { get { return _instance; } }
        private static readonly InverseNoItemsToVisibilityConverter _instance = new InverseNoItemsToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as IEnumerable;
            if (collection == null || !collection.Cast<object>().Any())
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
