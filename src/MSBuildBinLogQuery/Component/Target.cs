using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Target
    {
        public string Name { get; }
        public Project Parent { get; }
        public ConcurrentDictionary<int, Task> Tasks { get; }
        // TODO: Update the AddOrGetTask(int) method when PR #72 gets merged.
        public List<Task> OrderedTasks { get; }
        public TargetNode_BeforeThis Node_BeforeThis { get; }
        public TargetNode_AfterThis Node_AfterThis { get; }

        public Target(string name, Project parent) : base()
        {
            Name = name;
            Parent = parent;
            Tasks = new ConcurrentDictionary<int, Task>();
            OrderedTasks = new List<Task>();
            Node_BeforeThis = new TargetNode_BeforeThis(this);
            Node_AfterThis = new TargetNode_AfterThis(this);
        }
    }
}
