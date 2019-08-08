using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Component
{
    public class BuildResult : BuildComponent
    {
        public ConcurrentDictionary<int, Project> Projects { get; }
        public override BuildComponent Parent => null;

        public BuildResult() : base()
        {
            Projects = new ConcurrentDictionary<int, Project>();
        }
    }
}