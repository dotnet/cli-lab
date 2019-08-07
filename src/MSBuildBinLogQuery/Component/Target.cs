using System.Collections.Concurrent;
using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Target
    {
        public string Name { get; }
        public Project Parent { get; }
        public ConcurrentDictionary<int, Task> Tasks { get; }
        public TargetNode_BeforeThis Node_BeforeThis { get; }
        public TargetNode_AfterThis Node_AfterThis { get; }

        public Target(string name, Project parent) : base()
        {
            Name = name;
            Parent = parent;
            Tasks = new ConcurrentDictionary<int, Task>();
            Node_BeforeThis = new TargetNode_BeforeThis(this);
            Node_AfterThis = new TargetNode_AfterThis(this);
        }
    }
}
