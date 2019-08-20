using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class ComponentNode : AstNode, IEquatable<ComponentNode>
    {
        public AstNode Next { get; }
        public IReadOnlyList<ConstraintNode> Constraints => _constraints;

        private readonly List<ConstraintNode> _constraints;

        public ComponentNode(AstNode next, List<ConstraintNode> constraints = null) : base()
        {
            Next = next;
            _constraints = constraints ?? new List<ConstraintNode>();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ComponentNode);
        }

        public bool Equals([AllowNull] ComponentNode other)
        {
            return base.Equals(other) &&
                   EqualityComparer<AstNode>.Default.Equals(Next, other.Next) &&
                   Constraints.SequenceEqual(other.Constraints);
;        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(Next);

            foreach (var constraint in Constraints)
            {
                hashCode.Add(constraint);
            }

            return hashCode.ToHashCode();
        }
    }
}