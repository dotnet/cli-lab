using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Logging.Query.Result;

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

        public abstract IEnumerable<QueryResult> Interpret(IEnumerable<Component.Component> components);
    }
}