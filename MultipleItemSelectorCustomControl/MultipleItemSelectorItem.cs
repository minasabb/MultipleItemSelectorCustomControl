using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace MultipleItemSelectorCustomControl
{
    [TemplatePart(Name = PartNewItemText, Type = typeof(TextBox))]
    [TemplatePart(Name = PartTagBorder, Type = typeof(Border))]
    public class MultipleItemSelectorItem: ContentControl
    {
        private const string PartNewItemText = "PART_NewItemText";
        private const string PartTagBorder = "PART_TagBorder";
        private const int MaxNumBackKeyCount = 2;
        private int _backKeyCount;

        private bool _isLastItem;
        public bool IsLastItem
        {
            get { return _isLastItem; }
            set
            {
                if (_isLastItem != value)
                {
                    _isLastItem = value;
                    var NewItemText = GetTemplateChild(PartNewItemText) as FrameworkElement;
                    if (NewItemText != null)
                        NewItemText.Visibility = _isLastItem ? Visibility.Visible : Visibility.Collapsed;
                    
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
            var NewItemText = GetTemplateChild(PartNewItemText) as TextBox;
            if (NewItemText == null) return;
            NewItemText.KeyUp += TextboxOnPreviewKeyUp;
            NewItemText.Visibility = _isLastItem ? Visibility.Visible : Visibility.Collapsed;
            NewItemText.Focus();
        }

        void TextboxOnPreviewKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            var textbox = sender as TextBox;
            if(textbox==null || keyEventArgs==null)
                return;
            
            if ((keyEventArgs.Key == Key.Back && string.IsNullOrEmpty(textbox.Text)))
            {
                _backKeyCount++;
                if (_backKeyCount>= MaxNumBackKeyCount)
                {
                    var border = GetTemplateChild(PartTagBorder) as Border;
                    if (border != null) border.Focus();
                }
            }
            else if (keyEventArgs.Key == Key.Back && !string.IsNullOrEmpty(textbox.Text))
                _backKeyCount = 0;

        }


    }
}
