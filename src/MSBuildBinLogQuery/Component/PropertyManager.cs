using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Component
{
    public class PropertyManager
    {
        private readonly Dictionary<string, string> _properties;

        public PropertyManager()
        {
            _properties = new Dictionary<string, string>();
        }

        public void Set(string key, string value)
        {
            _properties[key] = value;
        }

        public bool TryGet(string key, out string value)
        {
            return _properties.TryGetValue(key, out value);
        }
    }
}
