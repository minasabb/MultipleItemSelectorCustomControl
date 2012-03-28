using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TagBoxCustomControl.ViewModel;

namespace MultipleItemSelectorCustomControl
{
    [TemplatePart(Name = PartNewItemText, Type = typeof(TextBox))]
    [TemplatePart(Name = PartSuggestionList, Type = typeof(ListBox))]
    public class MultipleItemSelectorAutoComplete: Control
    {
        private const string PartSuggestionList = "PART_SuggestionList";
        private const string PartNewItemText = "PART_NewItemText";
        IEnumerator<ItemViewModel> _matchingItemEnumerator;

        static MultipleItemSelectorAutoComplete()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultipleItemSelectorAutoComplete), new FrameworkPropertyMetadata(typeof(MultipleItemSelectorAutoComplete)));
        }

        public MultipleItemSelectorAutoComplete()
        {
            SetResourceReference(StyleProperty, "MultipleItemSelectorAutoCompleteStyle");
            KeyUp +=MultipleItemSelectorAutoCompleteKeyUp;
            
        }

        static void SuggestionlistPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listbox = sender as ListBox;
            if(listbox==null)
                return;
            var control = listbox.TemplatedParent as MultipleItemSelectorAutoComplete;
            if (control == null || string.IsNullOrEmpty( control.NewItemText))
                return;
            control.NewItemTextCompleted = true;
            control.NewItemText = string.Empty;
            control.IsSuggestionOpen = false;
        }

        void MultipleItemSelectorAutoCompleteKeyUp(object sender, KeyEventArgs e)
        {
            var suggestionlist = GetTemplateChild(PartSuggestionList) as ListBox;
            if (suggestionlist == null)
                return;
            if (e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.Return)
            {
                if (suggestionlist.Items.Count > 0 && !string.IsNullOrEmpty(NewItemText))
                {
                    NewItemTextCompleted = true;
                    NewItemText = string.Empty;
                    IsSuggestionOpen = false;
                }
            }
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                if (e.Key == Key.Up && suggestionlist.SelectedIndex > 0)
                    suggestionlist.SelectedIndex = suggestionlist.SelectedIndex - 1;
                if (e.Key == Key.Down && suggestionlist.SelectedIndex < suggestionlist.Items.Count)
                    suggestionlist.SelectedIndex = suggestionlist.SelectedIndex + 1;
            }
            e.Handled = true;
        }

        public static readonly DependencyProperty NewItemTextProperty =
            DependencyProperty.Register(
                "NewItemText",
                typeof(string),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnNewItemTextChanged));

        static void OnNewItemTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MultipleItemSelectorAutoComplete;
            if (control == null)
                return;

            control.IsSuggestionOpen = false;
            control.NewItemTextCompleted = false;
            var newValue = (string)e.NewValue;
            control.IsSuggestionOpen = !string.IsNullOrEmpty(newValue);

            if (!control.IsSuggestionOpen) return;
            //Update Filter
            var suggestionlist = control.GetTemplateChild(PartSuggestionList) as ListBox;
            if (suggestionlist == null)
                return;
            suggestionlist.PreviewMouseLeftButtonUp += SuggestionlistPreviewMouseLeftButtonUp;
            control.UpdateSuggestionList(suggestionlist,newValue);

        }

        void UpdateSuggestionList(ListBox suggestionlist,string filter)
        {
            //suggestionlist.Items.Filter = p =>
            //{
            //    var path = p as ItemViewModel;
            //    return filter != null && (path != null && (path.Name.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase) &&
            //                                                 !(String.Equals(path.Name, filter, StringComparison.CurrentCultureIgnoreCase))));
            //};
            var newFilteredList = new ObservableCollection<ItemViewModel>();
            if (_matchingItemEnumerator == null || !_matchingItemEnumerator.MoveNext())
                VerifyMatchingPeopleEnumerator(filter,SuggestionList.Cast<ItemViewModel>().FirstOrDefault());
            while (_matchingItemEnumerator != null && _matchingItemEnumerator.MoveNext())
            {
                newFilteredList.Add(_matchingItemEnumerator.Current);
            }
            suggestionlist.ItemsSource = newFilteredList;
            //If no items hide the suggestion
            if (suggestionlist.Items.Count == 0)
                IsSuggestionOpen = false;
            else
                suggestionlist.SelectedIndex = 0;
        }

        void VerifyMatchingPeopleEnumerator(string searchText,ItemViewModel item)
        {
            var matches = FindMatches(searchText, item);
            _matchingItemEnumerator = matches.GetEnumerator();
        }

        IEnumerable<ItemViewModel> FindMatches(string searchText, ItemViewModel item)
        {
            //if ((item.Name.StartsWith(searchText, StringComparison.CurrentCultureIgnoreCase) && 
            //    !(String.Equals(item.Name, searchText, StringComparison.CurrentCultureIgnoreCase))))
            //    yield return item;

            if (item.NameContainsText(searchText))
                yield return item;

            foreach (ItemViewModel match in item.Children.SelectMany(child => FindMatches(searchText, child)))
                yield return match;
        }

        public string NewItemText
        {
            get { return (string)GetValue(NewItemTextProperty); }
            set { SetValue(NewItemTextProperty, value); }
        }

        public static readonly DependencyProperty AddItemTextProperty =
            DependencyProperty.Register(
                "AddItemText",
                typeof(string),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string AddItemText
        {
            get { return (string)GetValue(AddItemTextProperty); }
            set { SetValue(AddItemTextProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                "Items",
                typeof(IEnumerable),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty SuggestionListProperty =
            DependencyProperty.Register(
                "SuggestionList",
                typeof(IEnumerable),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,OnSuggestionListChanged));

        static void OnSuggestionListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MultipleItemSelectorAutoComplete;
            if (control != null) control.FilteredSuggestionList = (IEnumerable) e.NewValue;
        }

        public IEnumerable SuggestionList
        {
            get { return (IEnumerable)GetValue(SuggestionListProperty); }
            set { SetValue(SuggestionListProperty, value); }
        }

        public static readonly DependencyProperty FilteredSuggestionListProperty =
            DependencyProperty.Register(
                "FilteredSuggestionList",
                typeof(IEnumerable),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable FilteredSuggestionList
        {
            get { return (IEnumerable)GetValue(FilteredSuggestionListProperty); }
            set { SetValue(FilteredSuggestionListProperty, value); }
        }

        public static readonly DependencyProperty SelectedSuggestionItemProperty =
            DependencyProperty.Register(
                "SelectedSuggestionItem",
                typeof(ItemViewModel),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ItemViewModel SelectedSuggestionItem
        {
            get { return (ItemViewModel)GetValue(SelectedSuggestionItemProperty); }
            set { SetValue(SelectedSuggestionItemProperty, value); }
        }

        public static readonly DependencyProperty IsSuggestionOpenProperty =
            DependencyProperty.Register(
                "IsSuggestionOpen",
                typeof(bool),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsSuggestionOpen
        {
            get { return (bool)GetValue(IsSuggestionOpenProperty); }
            set { SetValue(IsSuggestionOpenProperty, value); }
        }

        public static readonly DependencyProperty NewItemTextCompletedProperty =
            DependencyProperty.Register(
                "NewItemTextCompleted",
                typeof(bool),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool NewItemTextCompleted
        {
            get { return (bool)GetValue(NewItemTextCompletedProperty); }
            set { SetValue(NewItemTextCompletedProperty, value); }
        }
    }
}
