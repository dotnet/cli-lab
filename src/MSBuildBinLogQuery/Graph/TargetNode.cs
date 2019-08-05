using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode : IQueryableGraphNode
    {
        public string Name { get; }
        public ProjectNode Parent { get; }
        public ConcurrentHashSet<TaskNode> Tasks { get; }
        public ConcurrentHashSet<TargetNode> TargetsDirectlyBeforeThis { get; }
        public ConcurrentHashSet<TargetNode> TargetsDirectlyAfterThis { get; }

        public TargetNode(string name, ProjectNode parent) : base()
        {
            Name = name;
            Parent = parent;
            Tasks = new ConcurrentHashSet<TaskNode>();
            TargetsDirectlyBeforeThis = new ConcurrentHashSet<TargetNode>();
            TargetsDirectlyAfterThis = new ConcurrentHashSet<TargetNode>();
        }
    }
}
