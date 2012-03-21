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
    public class NoItemsToVisibilityConverter:IValueConverter
    {
        public static NoItemsToVisibilityConverter Instance { get { return _instance; } }
        private static readonly NoItemsToVisibilityConverter _instance = new NoItemsToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as IEnumerable;
            if (collection == null || !collection.Cast<object>().Any())
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
