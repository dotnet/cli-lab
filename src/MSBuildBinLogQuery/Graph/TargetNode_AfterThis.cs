using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode_AfterThis : IQueryableGraphNode<TargetNode>
    {
        public TargetNode TargetInfo { get; }
        public ConcurrentHashSet<TargetNode> AdjacentNodes { get; }

        public TargetNode_AfterThis(TargetNode targetInfo)
        {
            TargetInfo = targetInfo;
            AdjacentNodes = new ConcurrentHashSet<TargetNode>();
        }
    }
}
