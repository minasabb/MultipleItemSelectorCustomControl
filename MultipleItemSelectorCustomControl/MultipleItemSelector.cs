using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MultipleItemSelectorCustomControl
{
    [TemplatePart(Name = PartItemsBorder, Type = typeof(Border))]
    public class MultipleItemSelector:ItemsControl
    {
        private const string PartTagButton = "PART_TagButton";
        private const string PartNewItem = "PART_NewItem";
        private const string PartItemsBorder = "PART_itemsBorder";
        private const int MaxNumBackKeyCount = 2;
        private int _backKeyCount;
        private int _itemsCount;

        public MultipleItemSelector()
        {
            SetResourceReference(StyleProperty, "MultipleItemSelectorStyle");
            GotFocus += MultipleItemSelectorGotFocus;
        }

        void MultipleItemSelectorGotFocus(object sender, RoutedEventArgs e)
        {
            FindTextBoxControl(PartNewItem);
            FindButtonControl(PartTagButton);
        }

        void AddBorderEvents()
        {
            var mainBorder = GetTemplateChild(PartItemsBorder) as Border;
            if (mainBorder != null)
                mainBorder.MouseLeftButtonDown += MainBorderMouseLeftButtonDown;
            FindButtonControl(PartTagButton);
        }

        void MainBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FindTextBoxControl(PartNewItem);
        }

        public new Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public new static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(MultipleItemSelector), null);

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultipleItemSelectorItem();
        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultipleItemSelectorItem;
        }
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (ReferenceEquals(element, item))
                return;
            AddBorderEvents();
            var contentPresenter = element as ContentPresenter;
            ContentControl contentControl = null;
            if (contentPresenter == null)
            {
                contentControl = (element as ContentControl);
                if (contentControl == null)
                {
                    return;
                }
            }
            DataTemplate contentTemplate = null;
            if (!(item is UIElement))
                if (ItemTemplate != null)
                    contentTemplate = ItemTemplate;

            if (contentPresenter != null)
            {
                contentPresenter.Content = item;
                contentPresenter.ContentTemplate = contentTemplate;
            }
            else
            {
                contentControl.Content = item;
                contentControl.ContentTemplate = contentTemplate;
            }

            if (contentControl != null && (ItemContainerStyle != null && contentControl.Style == null))
                contentControl.Style = ItemContainerStyle;

            if (Items.Count > 0)
            {
                var container = element as MultipleItemSelectorItem;
                if (container == null)
                    return;
                if (ReferenceEquals(Items[Items.Count - 1], item) && _itemsCount == Items.Count-1)
                {
                    container.IsLastItem = true;
                    _itemsCount = 0;
                }
                else
                    _itemsCount++;
            }
            
        }
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (Items.Count > 1)
            {
                var container = (ItemContainerGenerator.ContainerFromIndex(Items.Count-1) as MultipleItemSelectorItem);
                if (container != null) container.IsLastItem = false;
            }
            if (Items.Count > 0)
            {
                var container = (ItemContainerGenerator.ContainerFromIndex(Items.Count-1) as MultipleItemSelectorItem);
                if (container != null) container.IsLastItem = true;
            }
        } 

        private void FindButtonControl(string name)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var container = ItemContainerGenerator.ContainerFromIndex(i);
                var button = GetChild<Button>(container);
                if (button == null || button.Name != name) continue;
                button.Click += ButtonOnClick;
            }
        }

        void ButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            
            var button = sender as Button;
            var itemIndex = -1;
            if(button==null)
                return;

            for (int i = 0; i < Items.Count; i++)
            {
                var container = ItemContainerGenerator.ContainerFromIndex(i);
                var currentButton = GetChild<Button>(container);

                if (currentButton != null && currentButton.Name == PartTagButton && currentButton.Content == button.Content)
                {
                    itemIndex = i;
                    break;
                }
            }
            if(itemIndex!=-1)
                DeleteItemByIndex(itemIndex);

            routedEventArgs.Handled = true;
        }

        private void FindTextBoxControl(string name)
        {
            var container = ItemContainerGenerator.ContainerFromIndex(Items.Count-1);
            var textbox = GetChild<TextBox>(container);
            if (textbox == null || textbox.Name != name) return;
            textbox.Visibility = Visibility.Visible;
            textbox.PreviewKeyUp += TextboxOnPreviewKeyUp;
            if(!textbox.IsFocused)
            textbox.Focus();
        }

        void TextboxOnPreviewKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter || keyEventArgs.Key == Key.Return || keyEventArgs.Key == Key.Tab)
                CreateNewItem();

            if (keyEventArgs.Key == Key.Back && string.IsNullOrEmpty(NewItem))
            {
                _backKeyCount++;
                if (_backKeyCount >= MaxNumBackKeyCount)
                {
                    DeletePreviousItem();
                    keyEventArgs.Handled = true;
                }
            }
            else if (keyEventArgs.Key == Key.Back && !string.IsNullOrEmpty(NewItem))
                _backKeyCount = 0;
        }

        public T GetChild<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject child = null;
            if (obj == null)
                return null;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child.GetType() == typeof(T))
                    break;
                if (child == null) continue;
                child = GetChild<T>(child);
                if (child != null && child.GetType() == typeof(T))
                    break;
            }
            return child as T;
        }

        private void CreateNewItem()
        {
            if (string.IsNullOrEmpty(NewItem)) return;
            ObservableCollection<object> currentItemSource=null;
            if(ItemsSource!=null)
                currentItemSource = new ObservableCollection<object>(ItemsSource.Cast<object>().ToList());
            if (currentItemSource!=null && currentItemSource.Any())
            {
                currentItemSource.Add(NewItem);
                ItemsSource = currentItemSource;
            }
            if (currentItemSource == null || !currentItemSource.Any())
                ItemsSource = new ObservableCollection<string> { NewItem };
            NewItem = string.Empty;
            _backKeyCount = 0;
        }

        private void DeletePreviousItem()
        {
            _backKeyCount = 0;
            var currentItemSource = new ObservableCollection<object>(ItemsSource.Cast<object>().ToList());
            if (currentItemSource.Any())
            {
                currentItemSource.RemoveAt(currentItemSource.Count-1);
                ItemsSource = currentItemSource;
                NewItem = string.Empty;
            }
        }

        private void DeleteItemByIndex(int index)
        {
            var currentItemSource = new ObservableCollection<object>(ItemsSource.Cast<object>().ToList());
            if (currentItemSource.Any() && index< currentItemSource.Count)
            {
                currentItemSource.RemoveAt(index);
                ItemsSource = currentItemSource;
                NewItem = string.Empty;
            }
        }

        public static readonly DependencyProperty NewItemProperty =
            DependencyProperty.Register(
                "NewItem",
                typeof(string),
                typeof(MultipleItemSelector),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string NewItem
        {
            get { return (string)GetValue(NewItemProperty); }
            set { SetValue(NewItemProperty, value); }
        }

        public static readonly DependencyProperty NewItemCompletedProperty =
            DependencyProperty.Register(
                "NewItemCompleted",
                typeof(bool),
                typeof(MultipleItemSelector),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,OnNewItemCompletedChanged));

        static void OnNewItemCompletedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var completed = (bool)e.NewValue;
            var control = d as MultipleItemSelector;
            if(control==null)
                return;
            if(completed)
                control.CreateNewItem();
            control.NewItemCompleted = false;
        }

        public bool NewItemCompleted
        {
            get { return (bool)GetValue(NewItemCompletedProperty); }
            set { SetValue(NewItemCompletedProperty, value); }
        }
    }
}
