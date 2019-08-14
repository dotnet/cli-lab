using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class TargetNode : QueryNode, IEquatable<TargetNode>
    {
        public TargetNode(QueryNode next) : base(next)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TargetNode);
        }

        public bool Equals([AllowNull] TargetNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}