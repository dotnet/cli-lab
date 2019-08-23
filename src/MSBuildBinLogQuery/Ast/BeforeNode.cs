using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class BeforeNode<TParent, TGraphNode> :
        DependencyNode<TParent, TGraphNode>,
        IEquatable<BeforeNode<TParent, TGraphNode>>
        where TParent : Component, IResultWithBeforeThis<TGraphNode>
        where TGraphNode : IDirectedAcyclicGraphNode<TGraphNode>, INodeWithComponent<TParent>
    {
        public BeforeNode(IAstNode<TParent> value, DependencyNodeType type) : base(value, type)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BeforeNode<TParent, TGraphNode>);
        }

        public bool Equals([AllowNull] BeforeNode<TParent, TGraphNode> other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<TParent> Filter(IEnumerable<TParent> components)
        {
            return components
                .SelectMany(component => component.Node_BeforeThis.AdjacentNodes.Select(node => node.Component));
        }
    }
}