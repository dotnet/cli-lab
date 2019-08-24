using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode_AfterThis :
        IDirectedAcyclicGraphNode<TargetNode_AfterThis>,
        IShallowCopyableGraphNode<TargetNode_AfterThis>,
        IEquatable<TargetNode_AfterThis>
    {
        public Target TargetInfo { get; }
        public ISet<TargetNode_AfterThis> AdjacentNodes { get; private set; }

        public TargetNode_AfterThis(Target targetInfo)
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

        public bool Equals([AllowNull] TargetNode_AfterThis other)
        {
            return other != null &&
                   EqualityComparer<Target>.Default.Equals(TargetInfo, other.TargetInfo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TargetInfo);
        }
    }
}