using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class WarningNode : LogNode, IEquatable<WarningNode>
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

        public override IEnumerable<QueryResult> Interpret(IEnumerable<Component.Component> components)
        {
            if (Type == LogNodeType.All)
            {
                return components.SelectMany(component => component.AllWarnings);
            }
            else if (Type == LogNodeType.Direct)
            {
                return components.SelectMany(Component => Component.Warnings);
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}