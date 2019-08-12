using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode_BeforeThis :
        IDirectedAcyclicGraphNode<TargetNode_BeforeThis>,
        IShallowCopyableGraphNode<TargetNode_BeforeThis>,
        IEquatable<TargetNode_BeforeThis>
    {
        public Target TargetInfo { get; }
        public ISet<TargetNode_BeforeThis> AdjacentNodes { get; private set; }

        public TargetNode_BeforeThis(Target targetInfo)
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

        public bool Equals([AllowNull] TargetNode_BeforeThis other)
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