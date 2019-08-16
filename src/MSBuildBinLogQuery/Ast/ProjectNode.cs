using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class ProjectNode : ComponentNode, IEquatable<ProjectNode>
    {
        public ProjectNode(AstNode next) : base(next)
        {
        }

        public ProjectNode(TaskNode next) : base(new TargetNode(next))
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

        public override IEnumerable<QueryResult> Interpret(IEnumerable<Component.Component> components)
        {
            var projects = Filter(components.Select(component => (Component.Build)component));
            return Next?.Interpret(projects) ?? projects;
        }

        private IEnumerable<Project> Filter(IEnumerable<Component.Build> builds)
        {
            return builds.SelectMany(build => build.ProjectsById.Values);
        }
    }
}