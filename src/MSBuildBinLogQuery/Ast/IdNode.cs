using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class IdNode<TParent, TBefore> : ConstraintNode<TParent, int, TBefore>, IEquatable<IdNode<TParent, TBefore>>
        where TParent : class, IQueryResult, IResultWithId
        where TBefore : class, IQueryResult
    {
        public IdNode(int value) : base(value)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdNode<TParent, TBefore>);
        }

        public bool Equals([AllowNull] IdNode<TParent, TBefore> other)
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
                .Where(component => component.Id == Value);
        }
    }
}