using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class MessageNode : LogNode, IEquatable<MessageNode>
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

        public override IEnumerable<QueryResult> Interpret(IEnumerable<Component.Component> components)
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