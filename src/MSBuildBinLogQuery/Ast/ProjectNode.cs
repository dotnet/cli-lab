using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class ProjectNode : ComponentNode, IEquatable<ProjectNode>
    {
        public ProjectNode(AstNode next) : base(next)
        {
        }

        public ProjectNode(TaskNode next) : base(new TargetNode(next))
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectNode);
        }

        public bool Equals([AllowNull] ProjectNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}