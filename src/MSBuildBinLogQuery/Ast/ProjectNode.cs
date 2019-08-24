using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class ProjectNode :
        ComponentNode<Project, Result.Build>,
        IEquatable<ProjectNode>
    {
        public ProjectNode(List<ConstraintNode<Project>> constraints = null) : base(null, constraints)
        {
        }

        public ProjectNode(IAstNode<Project> next, List<ConstraintNode<Project>> constraints = null) :
            base(next ?? throw new ArgumentNullException(), constraints)
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

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Result.Build> components)
        {
            var projects = components
                .SelectMany(build => build.ProjectsById.Values);

            var filteredProjects = FilterByConstraints(projects);

            return Next?.Filter(filteredProjects) ?? filteredProjects;
        }
    }
}