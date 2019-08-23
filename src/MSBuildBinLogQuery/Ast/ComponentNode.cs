using System;
using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class ComponentNode<TThis, TBefore> : AstNodeWithConstraints<TThis, TBefore>
        where TThis : Component
        where TBefore : Component
    {
        public IAstNode<TThis> Next { get; }

        public ComponentNode(IAstNode<TThis> next, List<ConstraintNode<TThis, TBefore>> constraints = null) :
            base(constraints)
        {
            Next = next;
        }

        protected bool Equals(ComponentNode<TThis, TBefore> other)
        {
            return base.Equals(other) &&
                   EqualityComparer<IAstNode<TThis>>.Default.Equals(Next, other.Next);
;        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetConstraintHashCode(), Next);
        }
    }
}