using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class WarningNode : QueryNode, IEquatable<WarningNode>
    {
        public WarningNode(QueryNode next) : base(next)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WarningNode);
        }

        public bool Equals([AllowNull] WarningNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}