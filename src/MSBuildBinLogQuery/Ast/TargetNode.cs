using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class TargetNode : ComponentNode<Target, Project>, IEquatable<TargetNode>
    {
        public TargetNode(List<ConstraintNode<Target>> constraints = null) : base(null, constraints)
        {
        }

        public TargetNode(IAstNode<Target> next, List<ConstraintNode<Target>> constraints = null) :
            base(next ?? throw new ArgumentNullException(), constraints)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TargetNode);
        }

        public bool Equals([AllowNull] TargetNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Project> components)
        {
            var targets = components
                .SelectMany(project => project.OrderedTargets);

            var filteredTargets = FilterByConstraints(targets);

            return Next?.Filter(filteredTargets) ?? filteredTargets;
        }
    }
}