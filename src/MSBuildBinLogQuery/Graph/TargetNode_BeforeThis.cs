using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode_BeforeThis : IQueryableGraphNode<TargetNode>
    {
        public TargetNode TargetInfo { get; }
        public ConcurrentHashSet<TargetNode> AdjacentNodes { get; }

        public TargetNode_BeforeThis(TargetNode targetInfo)
        {
            TargetInfo = targetInfo;
            AdjacentNodes = new ConcurrentHashSet<TargetNode>();
        }
    }
}
