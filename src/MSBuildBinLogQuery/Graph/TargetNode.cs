using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode : IQueryableGraphNode
    {
        public string Name { get; }
        public List<TaskNode> Tasks { get; }

        public TargetNode(string name) : base()
        {
            Name = name;
            Tasks = new List<TaskNode>();
        }
    }
}
