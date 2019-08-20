using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class ProjectNode : ComponentNode, IEquatable<ProjectNode>
    {
        public ProjectNode(List<ConstraintNode> constraints = null) : base(null, constraints)
        {
        }

        public ProjectNode(AstNode next, List<ConstraintNode> constraints = null) : base(next, constraints)
        {
        }

        public ProjectNode(TaskNode next, List<ConstraintNode> constraints = null) : base(new TargetNode(next), constraints)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectNode);
        }

        public bool Equals([AllowNull] ProjectNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Component.Component> components)
        {
            Debug.Assert(components.All(component => component is Component.Build));

            var projects = components
                .Select(component => component as Component.Build)
                .SelectMany(build => build.ProjectsById.Values);

            return Next?.Filter(projects) ?? projects;
        }
    }
}