using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class TaskNode : ComponentNode<Task, Target>, IEquatable<TaskNode>
    {
        public TaskNode(List<ConstraintNode<Task>> constraints = null) : base(null, constraints)
        {
        }

        public TaskNode(IAstNode<Task> next, List<ConstraintNode<Task>> constraints = null) :
            base(next ?? throw new ArgumentNullException(), constraints)
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

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Target> components)
        {
            var tasks = components
                .SelectMany(target => target.OrderedTasks);

            var filteredTasks = FilterByConstraints(tasks);

            return Next?.Filter(filteredTasks) ?? filteredTasks;
        }
    }
}