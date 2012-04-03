using System;
using System.Globalization;
using System.Windows.Data;
using MultipleItemSelectorCustomControl.ViewModel;

namespace MultipleItemSelectorCustomControl
{
    public class DisplayMemberPathConverter : IMultiValueConverter
    {
        public static DisplayMemberPathConverter Instance { get { return _instance; } }
        private static readonly DisplayMemberPathConverter _instance = new DisplayMemberPathConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 1)
                return null;
            var item = values[0] as ItemViewModel;
            var displayPath = string.Empty;
            if (values.Length > 1) 
                displayPath=values[1].ToString();
            else if (parameter!=null)
                displayPath = parameter.ToString();

            if (item !=null && !string.IsNullOrEmpty(displayPath))
                return item.GetType().GetProperty(displayPath).GetValue(item, null);
            return item != null ? item.ToString() : null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
