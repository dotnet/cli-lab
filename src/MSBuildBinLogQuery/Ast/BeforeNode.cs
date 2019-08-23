using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class BeforeNode<TParent, TGraphNode, TAstNode> :
        DependencyNode<TParent, TGraphNode, TAstNode>,
        IEquatable<BeforeNode<TParent, TGraphNode, TAstNode>>
        where TParent : Component, IResultWithBeforeThis<TGraphNode>
        where TGraphNode : IDirectedAcyclicGraphNode<TGraphNode>, INodeWithComponent<TParent>
        where TAstNode : IAstNode
    {
        public BeforeNode(TAstNode value, DependencyNodeType type) : base(value, type)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BeforeNode<TParent, TGraphNode, TAstNode>);
        }

        public bool Equals([AllowNull] BeforeNode<TParent, TGraphNode, TAstNode> other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<TParent> Filter(IEnumerable<TParent> components)
        {
            return Type switch
            {
                DependencyNodeType.All => throw new NotImplementedException(),
                DependencyNodeType.Direct => components.SelectMany(component => component.Node_BeforeThis.AdjacentNodes.Select(node => node.Component)),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}