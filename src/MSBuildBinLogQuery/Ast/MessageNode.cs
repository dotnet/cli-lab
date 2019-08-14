using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class MessageNode : QueryNode, IEquatable<MessageNode>
    {
        public MessageNode(QueryNode next) : base(next)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MessageNode);
        }

        public bool Equals([AllowNull] MessageNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}