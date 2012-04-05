using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using MultipleItemSelectorCustomControl.ViewModel;

namespace MultipleItemSelectorCustomControl
{
    [TemplatePart(Name = PartItemsBorder, Type = typeof(Border))]
    public class MultipleItemSelector:ItemsControl
    {
        private const string PartTagButton = "PART_TagButton";
        private const string PartNewItemText = "PART_NewItemText";
        private const string PartItemsBorder = "PART_itemsBorder";

        private int _itemsCount;

        public MultipleItemSelector()
        {
            SetResourceReference(StyleProperty, "MultipleItemSelectorStyle");
            MouseLeftButtonDown += MultipleItemSelectorMouseLeftButtonDown;
            GotFocus += MultipleItemSelectorGotFocus;
        }

        void MultipleItemSelectorMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            FindTextBoxControl(PartNewItemText);
            FindButtonControl(PartTagButton);
        }

        void MultipleItemSelectorGotFocus(object sender, RoutedEventArgs e)
        {
            FindButtonControl(PartTagButton);
        }

        void AddBorderEvents()
        {
            var mainBorder = GetTemplateChild(PartItemsBorder) as Border;
            if (mainBorder != null)
            {
                mainBorder.MouseLeftButtonDown += MainBorderMouseLeftButtonDown;
                mainBorder.PreviewKeyUp += MainBorderOnPreviewKeyUp;
            }
            FindButtonControl(PartTagButton);
        }

        void MainBorderOnPreviewKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Back && string.IsNullOrEmpty(NewItemText))
            {
                var container = ItemContainerGenerator.ContainerFromIndex(Items.Count - 1);
                var textbox = GetChild<TextBox>(container);
                if(textbox!=null && textbox.Visibility==Visibility.Visible && !textbox.IsFocused)
                    DeletePreviousItem();
            }
        }

        void MainBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FindTextBoxControl(PartNewItemText);
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

                if (currentButton != null && currentButton.Content is ItemViewModel && button.Content is ItemViewModel)
                {
                    var currentItem = currentButton.Content as ItemViewModel;
                    var selectedItem = button.Content as ItemViewModel;
                    if (selectedItem.Id == currentItem.Id)
                    {
                        itemIndex = i;
                        break;
                    }
                    
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
            if (!textbox.IsFocused)
                textbox.Focus();
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
            if (NewItem==null) return;
            ObservableCollection<ItemViewModel> currentItemSource=null;
            if(ItemsSource!=null)
                currentItemSource = new ObservableCollection<ItemViewModel>(ItemsSource.Cast<ItemViewModel>().ToList());
            if (currentItemSource!=null && currentItemSource.Any())
            {
                currentItemSource.Add(NewItem);
                ItemsSource = currentItemSource;
            }
            if (currentItemSource == null || !currentItemSource.Any())
                ItemsSource = new ObservableCollection<ItemViewModel> { NewItem };
            NewItemText = string.Empty;
            
        }

        private void DeletePreviousItem()
        {
            //_backKeyCount = 0;
            if (Items != null && Items.Count > 0)
                DeleteItemByIndex(Items.Count - 1);
        }

        private void DeleteItemByIndex(int index)
        {
            var currentItemSource = new ObservableCollection<ItemViewModel>(ItemsSource.Cast<ItemViewModel>().ToList());
            if (currentItemSource.Any() && index < currentItemSource.Count)
            {
                NewItemText = null;
                currentItemSource.RemoveAt(index);
                ItemsSource = currentItemSource;
                NewItemText = string.Empty;
            }
        }

        public static readonly DependencyProperty NewItemTextProperty =
            DependencyProperty.Register(
                "NewItemText",
                typeof(string),
                typeof(MultipleItemSelector),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string NewItemText
        {
            get { return (string)GetValue(NewItemTextProperty); }
            set { SetValue(NewItemTextProperty, value); }
        }

        public static readonly DependencyProperty NewItemProperty =
            DependencyProperty.Register(
                "NewItem",
                typeof(ItemViewModel),
                typeof(MultipleItemSelector),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ItemViewModel NewItem
        {
            get { return (ItemViewModel)GetValue(NewItemProperty); }
            set { SetValue(NewItemProperty, value); }
        }

        public static readonly DependencyProperty NewItemTextCompletedProperty =
            DependencyProperty.Register(
                "NewItemTextCompleted",
                typeof(bool),
                typeof(MultipleItemSelector),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,OnNewItemTextCompletedChanged));

        static void OnNewItemTextCompletedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var completed = (bool)e.NewValue;
            var control = d as MultipleItemSelector;
            if(control==null)
                return;
            if(completed)
                control.CreateNewItem();
            control.NewItemTextCompleted = false;
        }

        public bool NewItemTextCompleted
        {
            get { return (bool)GetValue(NewItemTextCompletedProperty); }
            set { SetValue(NewItemTextCompletedProperty, value); }
        }

    }
}
