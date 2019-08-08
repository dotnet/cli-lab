using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Utility
{
    public class ConcurrentHashSet<T> : IEnumerable<T>
    {
        private readonly ConcurrentDictionary<T, object> _dict;

        public ConcurrentHashSet()
        {
            _dict = new ConcurrentDictionary<T, object>();
        }

        public void Add(T item)
        {
            _dict.TryAdd(item, null);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }
    }
}
