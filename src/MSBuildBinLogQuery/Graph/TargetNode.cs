using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode
    {
        public string Name { get; }
        public ProjectNode Parent { get; }
        public ConcurrentHashSet<TaskNode> Tasks { get; }
        public TargetNode_BeforeThis Node_BeforeThis { get; }
        public TargetNode_AfterThis Node_AfterThis { get; }

        public TargetNode(string name, ProjectNode parent) : base()
        {
            Name = name;
            Parent = parent;
            Tasks = new ConcurrentHashSet<TaskNode>();
            Node_BeforeThis = new TargetNode_BeforeThis(this);
            Node_AfterThis = new TargetNode_AfterThis(this);
        }
    }
}
