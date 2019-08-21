using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class MessageNode : LogNode<Message>, IEquatable<MessageNode>
    {
        public MessageNode(LogNodeType type) : base(type)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MessageNode);
        }

        public bool Equals([AllowNull] MessageNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Component> components)
        {
            return FilterMessages(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Result.Build> components)
        {
            return FilterMessages(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Project> components)
        {
            return FilterMessages(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Target> components)
        {
            return FilterMessages(components);
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Task> components)
        {
            return FilterMessages(components);
        }

        private IEnumerable<IQueryResult> FilterMessages(IEnumerable<Component> components)
        {
            return Type switch
            {
                LogNodeType.All => components.SelectMany(component => component.AllMessages),
                LogNodeType.Direct => components.SelectMany(Component => Component.Messages),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}