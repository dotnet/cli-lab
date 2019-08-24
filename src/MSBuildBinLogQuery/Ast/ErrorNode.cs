using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class ErrorNode : LogNode<Error>, IEquatable<ErrorNode>
    {
        public ErrorNode(LogNodeType type) : base(type)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ErrorNode);
        }

        public bool Equals([AllowNull] ErrorNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Component> components)
        {
            return FilterErrors(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Result.Build> components)
        {
            return FilterErrors(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Project> components)
        {
            return FilterErrors(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Target> components)
        {
            return FilterErrors(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Task> components)
        {
            return FilterErrors(components);
        }

        private IEnumerable<IQueryResult> FilterErrors(IEnumerable<Component> components)
        {
            return Type switch
            {
                LogNodeType.All => components.SelectMany(component => component.AllErrors),
                LogNodeType.Direct => components.SelectMany(Component => Component.Errors),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}