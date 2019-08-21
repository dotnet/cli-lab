using System;
using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class ConstraintNode<TParent> : IAstNode, IFilterable<TParent, TParent>
        where TParent : class, IQueryResult
    {
        public ConstraintNode() : base()
        {
        }

        public abstract IEnumerable<TParent> Filter(IEnumerable<TParent> components);
    }

    public abstract class ConstraintNode<TParent, TValue> : ConstraintNode<TParent>
        where TParent : class, IQueryResult
    {
        public TValue Value { get; }

        public ConstraintNode(TValue value) : base()
        {
            Value = value;
        }

        protected bool Equals(ConstraintNode<TParent, TValue> other)
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