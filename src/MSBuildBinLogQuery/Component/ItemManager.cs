using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Component
{
    public class ItemManager
    {
        private readonly Dictionary<string, List<ITaskItem>> _items;

        public ItemManager()
        {
            _items = new Dictionary<string, List<ITaskItem>>();
        }

        public void Add(string name, ITaskItem taskItem)
        {
            if (!_items.ContainsKey(name))
            {
                _items[name] = new List<ITaskItem>();
            }

            _items[name].Add(taskItem);
        }

        public bool TryGet(string name, out List<ITaskItem> elements)
        {
            return _items.TryGetValue(name, out elements);
        }
    }
}
