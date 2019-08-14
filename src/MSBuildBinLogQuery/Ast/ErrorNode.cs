using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class ErrorNode : QueryNode, IEquatable<ErrorNode>
    {
        public ErrorNode(QueryNode next) : base(next)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ErrorNode);
        }

        public bool Equals([AllowNull] ErrorNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}