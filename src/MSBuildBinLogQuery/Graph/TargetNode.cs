using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode : IQueryableGraphNode
    {
        public string Name { get; }
        public ProjectNode Parent { get; }
        public HashSet<TaskNode> Tasks { get; }
        public HashSet<TargetNode> TargetsDirectlyBeforeThis { get; }
        public HashSet<TargetNode> TargetsDirectlyAfterThis { get; }

        public TargetNode(string name, ProjectNode parent) : base()
        {
            Name = name;
            Parent = parent;
            Tasks = new HashSet<TaskNode>();
            TargetsDirectlyBeforeThis = new HashSet<TargetNode>();
            TargetsDirectlyAfterThis = new HashSet<TargetNode>();
        }
    }
}
