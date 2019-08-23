using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class BeforeNode<TParent, TGraphNode, TAstNode, TBefore> :
        DependencyNode<TParent, TGraphNode, TAstNode, TBefore>,
        IEquatable<BeforeNode<TParent, TGraphNode, TAstNode, TBefore>>
        where TParent : Component, IResultWithBeforeThis<TGraphNode>
        where TGraphNode : IDirectedAcyclicGraphNode<TGraphNode>, INodeWithComponent<TParent>
        where TAstNode : IAstNode, IFilterable<TBefore, TParent>
        where TBefore : class, IQueryResult
    {
        public BeforeNode(TAstNode value, DependencyNodeType type) : base(value, type)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BeforeNode<TParent, TGraphNode, TAstNode, TBefore>);
        }

        public bool Equals([AllowNull] BeforeNode<TParent, TGraphNode, TAstNode, TBefore> other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<TParent> Filter(IEnumerable<TParent> components, IEnumerable<TBefore> previousComponents)
        {
            return Type switch
            {
                DependencyNodeType.All => throw new NotImplementedException(),
                DependencyNodeType.Direct => components.Intersect(Value.Filter(previousComponents)),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}