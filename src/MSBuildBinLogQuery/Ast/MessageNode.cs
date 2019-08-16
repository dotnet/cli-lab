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
            if (Type == LogNodeType.All)
            {
                return components.SelectMany(component => component.AllMessages);
            }
            else if (Type == LogNodeType.Direct)
            {
                return components.SelectMany(Component => Component.Messages);
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}