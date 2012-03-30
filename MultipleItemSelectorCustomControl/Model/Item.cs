using System.Collections.Generic;

namespace MultipleItemSelectorCustomControl.Model
{
    public class Item
    {
        readonly List<Item> _children = new List<Item>();
        public IList<Item> Children
        {
            get { return _children; }
        }

        public string Name { get; set; }

        public int Id { get; set; }
    }
}
