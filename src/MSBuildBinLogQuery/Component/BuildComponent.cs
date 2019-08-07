using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Component
{
    public abstract class BuildComponent
    {
        public IList<Message> Messages { get; }

        public BuildComponent()
        {
            Messages = new List<Message>();
        }
    }
}