using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MultipleItemSelectorCustomControl
{
    [TemplatePart(Name = PartNewItemText, Type = typeof(TextBox))]
    [TemplatePart(Name = PartTagBorder, Type = typeof(Border))]
    [TemplatePart(Name = PartChildrenPopup, Type = typeof(Popup))]
    [TemplatePart(Name = PartItemStackPanel, Type = typeof(StackPanel))]
    [TemplatePart(Name = PartChildrenList, Type = typeof(ListBox))]
    [TemplatePart(Name = PartExpanderText, Type = typeof(TextBlock))]
    public class MultipleItemSelectorItem: ContentControl
    {
        private const string PartNewItemText = "PART_NewItemText";
        private const string PartTagBorder = "PART_TagBorder";
        private const string PartChildrenPopup = "PART_ChildrenPopup";
        private const string PartItemStackPanel = "PART_ItemStackPanel";
        private const string PartChildrenList = "PART_ChildrenList";
        private const string PartExpanderText = "PART_ExpanderText";
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
                    var newItemText = GetTemplateChild(PartNewItemText) as FrameworkElement;
                    if (newItemText != null)
                        newItemText.Visibility = _isLastItem ? Visibility.Visible : Visibility.Collapsed;
                    
                }
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var newItemText = GetTemplateChild(PartNewItemText) as TextBox;
            if (newItemText == null) return;
            newItemText.KeyUp += TextboxOnPreviewKeyUp;
            newItemText.IsMouseCapturedChanged += NewItemTextOnIsMouseCapturedChanged;
            newItemText.Visibility = _isLastItem ? Visibility.Visible : Visibility.Collapsed;
            newItemText.Focus();
            var tagBorder = GetTemplateChild(PartTagBorder) as Border;
            if (tagBorder == null) return;
            tagBorder.MouseDown += TagBorderOnMouseDown;

            
            MouseLeave += OnLeaveMouse;
        }

        void NewItemTextOnIsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var textbox = sender as TextBox;
            if(textbox==null)
                return;
            IsSuggestionOpen = false;
            IsSuggestionOpen = true;
        }

        void OnLeaveMouse(object sender, RoutedEventArgs e)
        {
            var expanderText = GetTemplateChild(PartExpanderText) as TextBlock;
            if (expanderText == null) return;
            expanderText.Text = "+";
            CloseChildrenPopup();
        }

        void TagBorderOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var popup = GetTemplateChild(PartChildrenPopup) as Popup;
            if (popup == null) return;
            var expanderText = GetTemplateChild(PartExpanderText) as TextBlock;
            if (expanderText == null) return;
            if(popup.IsOpen)
            {
                CloseChildrenPopup();
                expanderText.Text = "+";
                return;
            }
            popup.IsOpen = true;
            popup.StaysOpen = true;
            
            expanderText.Text =  "-";
        }

        void TextboxOnPreviewKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            var textbox = sender as TextBox;
            if(textbox==null || keyEventArgs==null)
                return;

            CloseChildrenPopup();

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

        private void CloseChildrenPopup()
        {
            var popup = GetTemplateChild(PartChildrenPopup) as Popup;
            if (popup == null) return;
            popup.IsOpen = false;
            popup.StaysOpen = false;
        }

        public static readonly DependencyProperty IsSuggestionOpenProperty =
            DependencyProperty.Register(
                "IsSuggestionOpen",
                typeof(bool),
                typeof(MultipleItemSelectorItem),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsSuggestionOpen
        {
            get { return (bool)GetValue(IsSuggestionOpenProperty); }
            set
            {
                SetValue(IsSuggestionOpenProperty, value);
            }
        }
    }
}
