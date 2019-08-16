using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class TaskNode : ComponentNode, IEquatable<TaskNode>
    {
        public TaskNode(AstNode next) : base(next)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TaskNode);
        }

        public bool Equals([AllowNull] TaskNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}