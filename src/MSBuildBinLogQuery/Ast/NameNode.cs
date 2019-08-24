using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class NameNode<TParent> : ConstraintNode<TParent, string>, IEquatable<NameNode<TParent>>
        where TParent : class, IQueryResult, IResultWithName
    {
        public NameNode(string value) : base(value)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NameNode<TParent>);
        }

        public bool Equals([AllowNull] NameNode<TParent> other)
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
                .Where(component => component.Name == Value);
        }
    }
}