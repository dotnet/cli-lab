using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class NameNode<TParent, TBefore> : ConstraintNode<TParent, string, TBefore>, IEquatable<NameNode<TParent, TBefore>>
        where TParent : class, IQueryResult, IResultWithName
        where TBefore : class, IQueryResult
    {
        public NameNode(string value) : base(value)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NameNode<TParent, TBefore>);
        }

        public bool Equals([AllowNull] NameNode<TParent, TBefore> other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<TParent> Filter(IEnumerable<TParent> components, IEnumerable<TBefore> previousComponents)
        {
            return components
                .Where(component => component.Name == Value);
        }
    }
}