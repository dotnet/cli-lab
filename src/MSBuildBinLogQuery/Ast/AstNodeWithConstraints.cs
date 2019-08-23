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
        public IReadOnlyList<ConstraintNode<TThis, TBefore>> Constraints => _constraints;

        private readonly List<ConstraintNode<TThis, TBefore>> _constraints;

        public AstNodeWithConstraints(List<ConstraintNode<TThis, TBefore>> constraints = null) : base()
        {
            _constraints = constraints ?? new List<ConstraintNode<TThis, TBefore>>();
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

        protected IEnumerable<TThis> FilterByConstraints(IEnumerable<TThis> components, IEnumerable<TBefore> previousComponents)
        {
            foreach (var constraint in Constraints)
            {
                components = constraint.Filter(components, previousComponents);
            }

            return components;
        }

        public abstract IEnumerable<IQueryResult> Filter(IEnumerable<TBefore> components);
    }
}