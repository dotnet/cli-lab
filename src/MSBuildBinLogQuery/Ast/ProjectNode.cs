using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public sealed class ProjectNode :
        ComponentNode<Project, Result.Build>,
        IEquatable<ProjectNode>,
        IFilterable<Result.Build, Project>
    {
        public ProjectNode(List<ConstraintNode<Project, Result.Build>> constraints = null) : base(null, constraints)
        {
        }

        public ProjectNode(IAstNode<Project> next, List<ConstraintNode<Project, Result.Build>> constraints = null) :
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
            var filteredProjects = FilterProject(components);

            return Next?.Filter(filteredProjects) ?? filteredProjects;
        }

        IEnumerable<Project> IFilterable<Result.Build, Project>.Filter(IEnumerable<Result.Build> components)
        {
            return FilterProject(components);
        }

        private IEnumerable<Project> FilterProject(IEnumerable<Result.Build> components)
        {
            var projects = components
                .SelectMany(build => build.ProjectsById.Values)
                .Distinct();

            return FilterByConstraints(projects, components);
        }
    }
}