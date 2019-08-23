using System;
using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class ConstraintNode<TParent, TBefore> : IAstNode, IFilterable<TParent, TParent, TBefore>
        where TParent : class, IQueryResult
        where TBefore : class, IQueryResult
    {
        public ConstraintNode()
        {
        }

        public abstract IEnumerable<TParent> Filter(IEnumerable<TParent> components, IEnumerable<TBefore> previousComponents);
    }

    public abstract class ConstraintNode<TParent, TValue, TBefore> : ConstraintNode<TParent, TBefore>
        where TParent : class, IQueryResult
        where TBefore : class, IQueryResult
    {
        public TValue Value { get; }

        public ConstraintNode(TValue value) : base()
        {
            Value = value;
        }

        protected bool Equals(ConstraintNode<TParent, TValue, TBefore> other)
        {
            return other != null &&
                   EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}