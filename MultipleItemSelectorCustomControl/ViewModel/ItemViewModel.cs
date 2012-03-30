using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MultipleItemSelectorCustomControl.Model;

namespace MultipleItemSelectorCustomControl.ViewModel
{
    public class ItemViewModel : INotifyPropertyChanged
    {

        readonly ObservableCollection<ItemViewModel> _children;
        readonly ItemViewModel _parent;
        readonly Item _item;

        //bool _isExpanded;
        bool _isSelected;

        public ItemViewModel(Item item)
            : this(item, null)
        {
        }

        private ItemViewModel(Item item, ItemViewModel parent)
        {
            IsSelected = true;
            if (item != null)
            {
                _item = item;
                _parent = parent;

                _children = new ObservableCollection<ItemViewModel>(
                    (from child in _item.Children
                     select new ItemViewModel(child, this))
                        .ToList<ItemViewModel>());
            }
        }

        public ObservableCollection<ItemViewModel> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _item.Name; }
        }

        public int Id
        {
            get { return _item.Id; }
        }

        //public bool IsExpanded
        //{
        //    get { return _isExpanded; }
        //    set
        //    {
        //        if (value != _isExpanded)
        //        {
        //            _isExpanded = value;
        //            this.OnPropertyChanged("IsExpanded");
        //        }

        //        // Expand all the way up to the root.
        //        if (_isExpanded && _parent != null)
        //            _parent.IsExpanded = true;
        //    }
        //}

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(Name))
                return false;

            return Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        public ItemViewModel Parent
        {
            get { return _parent; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
