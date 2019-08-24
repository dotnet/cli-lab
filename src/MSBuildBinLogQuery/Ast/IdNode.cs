using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class IdNode<TParent> : ConstraintNode<TParent, int>, IEquatable<IdNode<TParent>>
        where TParent : class, IQueryResult, IResultWithId
    {
        public IdNode(int value) : base(value)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdNode<TParent>);
        }

        public bool Equals([AllowNull] IdNode<TParent> other)
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
                .Where(component => component.Id == Value);
        }
    }
}