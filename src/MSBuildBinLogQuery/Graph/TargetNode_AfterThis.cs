using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode_AfterThis :
        IQueryableGraphNode<TargetNode_AfterThis>,
        IShallowCopyableGraphNode<TargetNode_AfterThis>
    {
        public TargetNode TargetInfo { get; }
        public ISet<TargetNode_AfterThis> AdjacentNodes { get; private set; }

        public TargetNode_AfterThis(TargetNode targetInfo)
        {
            TargetInfo = targetInfo;
            AdjacentNodes = new HashSet<TargetNode_AfterThis>();
        }

        public TargetNode_AfterThis ShallowCopyAndClearEdges()
        {
            var copy = MemberwiseClone() as TargetNode_AfterThis;
            copy.AdjacentNodes = new HashSet<TargetNode_AfterThis>();
            return copy;
        }
    }
}
