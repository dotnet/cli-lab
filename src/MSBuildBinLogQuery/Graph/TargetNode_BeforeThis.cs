using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode_BeforeThis :
        IDirectedAcyclicGraphNode<TargetNode_BeforeThis>,
        IShallowCopyableGraphNode<TargetNode_BeforeThis>,
        INodeWithComponent<Target>,
        IEquatable<TargetNode_BeforeThis>
    {
        public Target Component { get; }
        public ISet<TargetNode_BeforeThis> AdjacentNodes { get; private set; }

        public TargetNode_BeforeThis(Target targetInfo)
        {
            Component = targetInfo;
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
                   EqualityComparer<Target>.Default.Equals(Component, other.Component);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Component);
        }
    }
}