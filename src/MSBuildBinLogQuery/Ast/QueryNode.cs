using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class QueryNode : IEquatable<QueryNode>
    {
        public QueryNode Next { get; }

        public QueryNode(QueryNode next)
        {
            Next = next;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as QueryNode);
        }

        public bool Equals([AllowNull] QueryNode other)
        {
            return other != null &&
                   EqualityComparer<QueryNode>.Default.Equals(Next, other.Next);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Next);
        }
    }
}