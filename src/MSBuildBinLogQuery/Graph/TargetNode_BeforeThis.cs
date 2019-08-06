using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode_BeforeThis :
        IQueryableGraphNode<TargetNode_BeforeThis>,
        IShallowCopyableGraphNode<TargetNode_BeforeThis>
    {
        public TargetNode TargetInfo { get; }
        public ISet<TargetNode_BeforeThis> AdjacentNodes { get; private set; }

        public TargetNode_BeforeThis(TargetNode targetInfo)
        {
            TargetInfo = targetInfo;
            AdjacentNodes = new HashSet<TargetNode_BeforeThis>();
        }

        public TargetNode_BeforeThis ShallowCopyAndClearEdges()
        {
            var copy = MemberwiseClone() as TargetNode_BeforeThis;
            copy.AdjacentNodes = new HashSet<TargetNode_BeforeThis>();
            return copy;
        }
    }
}
