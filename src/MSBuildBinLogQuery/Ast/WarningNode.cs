using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class WarningNode : LogNode<Warning>, IEquatable<WarningNode>
    {
        public WarningNode(LogNodeType type) : base(type)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WarningNode);
        }

        public bool Equals([AllowNull] WarningNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Component> components)
        {
            return FilterWarnings(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Result.Build> components)
        {
            return FilterWarnings(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Project> components)
        {
            return FilterWarnings(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Target> components)
        {
            return FilterWarnings(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Task> components)
        {
            return FilterWarnings(components);
        }

        private IEnumerable<IQueryResult> FilterWarnings(IEnumerable<Component> components)
        {
            return Type switch
            {
                LogNodeType.All => components.SelectMany(component => component.AllWarnings),
                LogNodeType.Direct => components.SelectMany(Component => Component.Warnings),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}