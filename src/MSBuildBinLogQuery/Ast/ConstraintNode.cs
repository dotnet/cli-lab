using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class ConstraintNode : AstNode
    {
        public ConstraintNode() : base()
        {
        }
    }

    public abstract class ConstraintNode<TValue> : ConstraintNode, IEquatable<ConstraintNode<TValue>>
    {
        public TValue Value { get; }

        public ConstraintNode(TValue value) : base()
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdNode);
        }

        public bool Equals([AllowNull] ConstraintNode<TValue> other)
        {
            return base.Equals(other) &&
                   EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}