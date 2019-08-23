using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TargetNode_AfterThis :
        IDirectedAcyclicGraphNode<TargetNode_AfterThis>,
        IShallowCopyableGraphNode<TargetNode_AfterThis>,
        INodeWithComponent<Target>,
        IEquatable<TargetNode_AfterThis>
    {
        public Target Component { get; }
        public ISet<TargetNode_AfterThis> AdjacentNodes { get; private set; }

        public TargetNode_AfterThis(Target targetInfo)
        {
            Component = targetInfo;
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
                   EqualityComparer<Target>.Default.Equals(Component, other.Component);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Component);
        }
    }
}