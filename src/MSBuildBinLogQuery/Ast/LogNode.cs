using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class LogNode : AstNode, IEquatable<LogNode>
    {
        public LogNodeType Type { get; }

        public LogNode(LogNodeType type) : base()
        {
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LogNode);
        }

        public bool Equals([AllowNull] LogNode other)
        {
            return base.Equals(other) &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);
        }
    }
}