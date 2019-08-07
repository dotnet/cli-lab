using System.Collections.Concurrent;
using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Utility
{
    public class ItemManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<ITaskItem>> _items;

        public ItemManager()
        {
            _items = new ConcurrentDictionary<string, ConcurrentBag<ITaskItem>>();
        }

        public void Add(string name, ITaskItem taskItem)
        {
            var item = _items.GetOrAdd(name, new ConcurrentBag<ITaskItem>());
            item.Add(taskItem);
        }

        public bool TryGet(string name, out ConcurrentBag<ITaskItem> elements)
        {
            return _items.TryGetValue(name, out elements);
        }
    }
}
