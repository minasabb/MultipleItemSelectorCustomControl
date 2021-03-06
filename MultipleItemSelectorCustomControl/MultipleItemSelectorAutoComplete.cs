﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MultipleItemSelectorCustomControl.ViewModel;

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

        void TextboxIsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsSuggestionOpen = true;
        }

        static void SuggestionlistPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listbox = sender as ListBox;
            if(listbox==null)
                return;
            var control = listbox.TemplatedParent as MultipleItemSelectorAutoComplete;
            if (control == null)
                return;
            control.AddNewItem(listbox);
        }

        void MultipleItemSelectorAutoCompleteKeyUp(object sender, KeyEventArgs e)
        {
            var suggestionlist = GetTemplateChild(PartSuggestionList) as ListBox;
            if (suggestionlist == null)
                return;
            if (e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.Return)
            {
                AddNewItem(suggestionlist);
            }
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                if(string.IsNullOrEmpty(NewItemText))
                {
                    IsSuggestionOpen = true;
                    //if (suggestionlist.Items.Count<2)
                    //    UpdateSuggestionList("");
                }
                if (e.Key == Key.Up && suggestionlist.SelectedIndex > 0)
                    suggestionlist.SelectedIndex = suggestionlist.SelectedIndex - 1;
                if (e.Key == Key.Down && suggestionlist.SelectedIndex < suggestionlist.Items.Count)
                    suggestionlist.SelectedIndex = suggestionlist.SelectedIndex + 1;
            }
            e.Handled = true;
        }

        void AddNewItem(ListBox suggestionlist)
        {
            if (suggestionlist.Items.Count > 0 && suggestionlist.SelectedIndex != -1) //&& !string.IsNullOrEmpty(NewItemText))
            {
                IsSuggestionOpen = false;
                NewItemText = ((ItemViewModel) suggestionlist.SelectedItem).Name;
                NewItemTextCompleted = true;
                NewItemText = string.Empty;
                suggestionlist.SelectedIndex = -1;
                
                NewItemTextCompleted = false;
            }
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
            control.IsSuggestionOpen = true;//!string.IsNullOrEmpty(newValue);

            //if (!control.IsSuggestionOpen) return;
            //Update Filter
            control.UpdateSuggestionList(newValue);
        }

        private void UpdateSuggestionList(string filter)
        {
            var suggestionlist = GetTemplateChild(PartSuggestionList) as ListBox;
            if (suggestionlist == null)
                return;
            suggestionlist.PreviewMouseLeftButtonUp += SuggestionlistPreviewMouseLeftButtonUp;
            UpdateSuggestionList(suggestionlist, filter);
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
            foreach (var itemViewModel in SuggestionList)
            {
                if (_matchingItemEnumerator == null || !_matchingItemEnumerator.MoveNext())
                    VerifyMatchingPeopleEnumerator(filter, (ItemViewModel) itemViewModel);
                while (_matchingItemEnumerator != null && _matchingItemEnumerator.MoveNext())
                {
                    if (Items == null || Items.Cast<ItemViewModel>().All(item => item.Id != _matchingItemEnumerator.Current.Id))
                        newFilteredList.Add(_matchingItemEnumerator.Current);
                }
            }
            
            FilteredSuggestionList = newFilteredList;

            //If no items hide the suggestion
            if (!newFilteredList.Any())
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

            if (item.NameContainsText(searchText) || string.IsNullOrEmpty(searchText))
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
            if (control == null)
                return;
                //control.FilteredSuggestionList = (IEnumerable) e.NewValue;
             control.UpdateSuggestionList("");

            var textbox = control.GetTemplateChild(PartNewItemText) as TextBox;
            if (textbox == null)
                return;
            textbox.IsMouseCapturedChanged += control.TextboxIsMouseCapturedChanged;
           
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
            set
            {
                SetValue(IsSuggestionOpenProperty, value);
            }
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

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
    }
}
