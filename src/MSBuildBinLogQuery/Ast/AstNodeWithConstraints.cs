using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class AstNodeWithConstraints<TThis, TBefore> :
        IAstNode<TThis, TBefore>
        where TThis : class, IQueryResult
        where TBefore : class, IQueryResult
    {
        public IReadOnlyList<ConstraintNode<TThis>> Constraints => _constraints;

        private readonly List<ConstraintNode<TThis>> _constraints;

        public AstNodeWithConstraints(List<ConstraintNode<TThis>> constraints = null) : base()
        {
            _constraints = constraints ?? new List<ConstraintNode<TThis>>();
        }

        protected bool Equals(AstNodeWithConstraints<TThis, TBefore> other)
        {
            return other != null &&
                   Constraints.SequenceEqual(other.Constraints);
;        }

        protected int GetConstraintHashCode()
        {
            var hashCode = new HashCode();

            foreach (var constraint in Constraints)
            {
                hashCode.Add(constraint);
            }

            return hashCode.ToHashCode();
        }

        protected IEnumerable<TThis> FilterByConstraints(IEnumerable<TThis> components)
        {
            foreach (var constraint in Constraints)
            {
                components = constraint.Filter(components);
            }

            return components;
        }

        public abstract IEnumerable<IQueryResult> Filter(IEnumerable<TBefore> components);
    }
}