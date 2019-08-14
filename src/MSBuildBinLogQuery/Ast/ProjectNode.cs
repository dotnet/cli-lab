using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class ProjectNode : QueryNode, IEquatable<ProjectNode>
    {
        public ProjectNode(QueryNode next) : base(next)
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