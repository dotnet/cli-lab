using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class TaskNode : ComponentNode, IEquatable<TaskNode>
    {
        public TaskNode(List<ConstraintNode> constraints = null) : base(null, constraints)
        {
        }

        public TaskNode(AstNode next, List<ConstraintNode> constraints = null) :
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

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Component.Component> components)
        {
            Debug.Assert(components.All(component => component is Target));

            var tasks = components
                .Select(component => component as Target)
                .SelectMany(target => target.OrderedTasks);

            return Next?.Filter(tasks) ?? tasks;
        }
    }
}