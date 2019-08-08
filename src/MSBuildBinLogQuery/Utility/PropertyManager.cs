using System.Collections.Concurrent;

namespace Microsoft.Build.Logging.Query.Utility
{
    public class PropertyManager
    {
        private readonly ConcurrentDictionary<string, string> _properties;

        public PropertyManager()
        {
            _properties = new ConcurrentDictionary<string, string>();
        }

        public void Set(string key, string value)
        {
            _properties.AddOrUpdate(key, k => value, (k, v) => value);
        }

        public bool TryGet(string key, out string value)
        {
            return _properties.TryGetValue(key, out value);
        }
    }
}
