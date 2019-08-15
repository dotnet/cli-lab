using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class AstNode : IEquatable<AstNode>
    {
        public AstNode()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AstNode);
        }

        public bool Equals([AllowNull] AstNode other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}