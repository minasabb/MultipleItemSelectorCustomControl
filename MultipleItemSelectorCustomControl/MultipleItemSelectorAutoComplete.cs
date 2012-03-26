﻿using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultipleItemSelectorCustomControl
{
    [TemplatePart(Name = PartNewItem, Type = typeof(TextBox))]
    [TemplatePart(Name = PartSuggestionList, Type = typeof(ListBox))]
    public class MultipleItemSelectorAutoComplete: Control
    {
        private const string PartSuggestionList = "PART_SuggestionList";
        private const string PartNewItem = "PART_NewItem";

        static MultipleItemSelectorAutoComplete()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultipleItemSelectorAutoComplete), new FrameworkPropertyMetadata(typeof(MultipleItemSelectorAutoComplete)));
        }

        public MultipleItemSelectorAutoComplete()
        {
            SetResourceReference(StyleProperty, "MultipleItemSelectorAutoCompleteStyle");
            KeyUp +=MultipleItemSelectorAutoCompleteKeyUp;
        }

        void MultipleItemSelectorAutoCompleteKeyUp(object sender, KeyEventArgs e)
        {
            var suggestionlist = GetTemplateChild(PartSuggestionList) as ListBox;
            if (suggestionlist == null)
                return;
            if (e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.Return)
            {
                if (suggestionlist.Items.Count > 0)
                {
                    NewItem = SelectedSuggestionItem;
                    NewItemCompleted = true;
                    NewItem = string.Empty;
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

        public static readonly DependencyProperty NewItemProperty =
            DependencyProperty.Register(
                "NewItem",
                typeof(string),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnNewItemChanged));

        static void OnNewItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MultipleItemSelectorAutoComplete;
            if (control == null)
                return;
            control.NewItemCompleted = false;
            var newValue = (string)e.NewValue;
            control.IsSuggestionOpen = !string.IsNullOrEmpty(newValue);

            if (!control.IsSuggestionOpen) return;
            //Update Filter
            var suggestionlist = control.GetTemplateChild(PartSuggestionList) as ListBox;
            if (suggestionlist == null)
                return;
            control.UpdateSuggestionList(suggestionlist,newValue);

        }

        void UpdateSuggestionList(ListBox suggestionlist,string filter)
        {
            suggestionlist.Items.Filter = p =>
            {
                var path = p as string;
                return filter != null && (path != null && (path.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase) &&
                                                             !(String.Equals(path, filter, StringComparison.CurrentCultureIgnoreCase))));
            };

            //If no items hide the suggestion
            if (suggestionlist.Items.Count == 0)
                IsSuggestionOpen = false;
            else
                suggestionlist.SelectedIndex = 0;
        }

        public string NewItem
        {
            get { return (string)GetValue(NewItemProperty); }
            set { SetValue(NewItemProperty, value); }
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
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable SuggestionList
        {
            get { return (IEnumerable)GetValue(SuggestionListProperty); }
            set { SetValue(SuggestionListProperty, value); }
        }

        public static readonly DependencyProperty SelectedSuggestionItemProperty =
            DependencyProperty.Register(
                "SelectedSuggestionItem",
                typeof(string),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSuggestionSelected));

        static void OnSuggestionSelected(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MultipleItemSelectorAutoComplete;
            if (control == null)
                return;
            var suggestionlist = control.GetTemplateChild(PartSuggestionList) as ListBox;
            if (suggestionlist == null || suggestionlist.SelectedItem==null)
                    return;
            if (suggestionlist.IsMouseCaptureWithin && control.SelectedSuggestionItem!=null)
            {
                control.NewItem = control.SelectedSuggestionItem;
                control.NewItemCompleted = true;
                control.NewItem = string.Empty;
            }
            
        }

        public string SelectedSuggestionItem
        {
            get { return (string)GetValue(SelectedSuggestionItemProperty); }
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

        public static readonly DependencyProperty NewItemCompletedProperty =
            DependencyProperty.Register(
                "NewItemCompleted",
                typeof(bool),
                typeof(MultipleItemSelectorAutoComplete),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool NewItemCompleted
        {
            get { return (bool)GetValue(NewItemCompletedProperty); }
            set { SetValue(NewItemCompletedProperty, value); }
        }
    }
}
