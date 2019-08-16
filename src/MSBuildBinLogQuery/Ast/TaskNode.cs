using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class TaskNode : ComponentNode, IEquatable<TaskNode>
    {
        public TaskNode(AstNode next) : base(next)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TaskNode);
        }

        public bool Equals([AllowNull] TaskNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<QueryResult> Interpret(IEnumerable<Component.Component> components)
        {
            var tasks = Filter(components.Select(component => (Target)component));
            return Next?.Interpret(tasks) ?? tasks;
        }

        private IEnumerable<Task> Filter(IEnumerable<Target> targets)
        {
            return targets.SelectMany(target => target.OrderedTasks);
        }
    }
}