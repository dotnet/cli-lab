using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class IdNode : ConstraintNode<int>, IEquatable<IdNode>
    {
        public IdNode(int value) : base(value)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdNode);
        }

        public bool Equals([AllowNull] IdNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Component.Component> components)
        {
            Debug.Assert(components.All(component => component is IComponentWithId));

            return components
                .Select(component => component as IComponentWithId)
                .Where(component => component.Id == Value);
        }
    }
}