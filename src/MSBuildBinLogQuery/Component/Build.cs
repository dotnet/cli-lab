using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Build : ILoggable
    {
        public ConcurrentDictionary<int, Project> Projects { get; }
        public IList<Message> Messages { get; }

        public Build()
        {
            Projects = new ConcurrentDictionary<int, Project>();
            Messages = new List<Message>();
        }
    }
}