using System.Windows.Controls;
using System.Windows;

namespace MultipleItemSelectorCustomControl
{
    [TemplatePart(Name = PartNewItem, Type = typeof(TextBox))]
    public class MultipleItemSelectorItem: ContentControl
    {
        private const string PartNewItem = "PART_NewItem";

        private bool _isLastItem;
        public bool IsLastItem
        {
            get { return _isLastItem; }
            set
            {
                if (_isLastItem != value)
                {
                    _isLastItem = value;
                    var newItem = GetTemplateChild(PartNewItem) as FrameworkElement;
                    if (newItem != null)
                        newItem.Visibility = _isLastItem ? Visibility.Visible : Visibility.Collapsed;
                    
                }
            }
        }
        public bool IsEmptyItem
        {
            get;
            set;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var newItem = GetTemplateChild(PartNewItem) as FrameworkElement;
            if (newItem == null) return;
            newItem.Visibility = _isLastItem ? Visibility.Visible : Visibility.Collapsed;
            newItem.Focus();
        } 

    }
}
