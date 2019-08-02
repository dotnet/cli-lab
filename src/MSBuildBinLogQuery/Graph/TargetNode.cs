using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode : IQueryableGraphNode
    {
        public string Name { get; }
        public ProjectNode Parent { get; }
        public HashSet<TaskNode> Tasks { get; }
        public HashSet<TargetNode> TargetsBeforeThis { get; }
        public HashSet<TargetNode> TargetsAfterThis { get; }

        public TargetNode(string name, ProjectNode parent) : base()
        {
            Name = name;
            Parent = parent;
            Tasks = new HashSet<TaskNode>();
            TargetsBeforeThis = new HashSet<TargetNode>();
            TargetsAfterThis = new HashSet<TargetNode>();
        }
    }
}
