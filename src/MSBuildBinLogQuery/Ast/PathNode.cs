using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class PathNode<TParent> : ConstraintNode<TParent, string>, IEquatable<PathNode<TParent>>
        where TParent : class, IQueryResult, IResultWithPath
    {
        public PathNode(string value) : base(value)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PathNode<TParent>);
        }

        public bool Equals([AllowNull] PathNode<TParent> other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<TParent> Filter(IEnumerable<TParent> components)
        {
            return components
                .Where(component => component.Path == Value);
        }
    }
}