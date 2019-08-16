using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class ComponentNode : AstNode, IEquatable<ComponentNode>
    {
        public AstNode Next { get; }

        public ComponentNode(AstNode next) : base()
        {
            Next = next;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ComponentNode);
        }

        public bool Equals([AllowNull] ComponentNode other)
        {
            return base.Equals(other) &&
                   EqualityComparer<AstNode>.Default.Equals(Next, other.Next);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Next);
        }
    }
}