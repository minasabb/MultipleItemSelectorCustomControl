using System;
using System.Globalization;
using System.Windows.Data;
using MultipleItemSelectorCustomControl.ViewModel;

namespace MultipleItemSelectorCustomControl
{
    public class DisplayMemberPathConverter : IValueConverter
    {
        public static DisplayMemberPathConverter Instance { get { return _instance; } }
        private static readonly DisplayMemberPathConverter _instance = new DisplayMemberPathConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as ItemViewModel;
            var displayPath = parameter.ToString();
            if (item !=null && !string.IsNullOrEmpty(displayPath))
                return item.GetType().GetProperty(displayPath).GetValue(item, null);
            if (item != null) return item.ToString();
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
