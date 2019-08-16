using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class TargetNode : ComponentNode, IEquatable<TargetNode>
    {
        public TargetNode(AstNode next) : base(next)
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

        public override IEnumerable<QueryResult> Interpret(IEnumerable<Component.Component> components)
        {
            var targets = Filter(components.Select(component => (Project)component));
            return Next?.Interpret(targets) ?? targets;
        }

        private IEnumerable<Target> Filter(IEnumerable<Project> projects)
        {
            return projects.SelectMany(project => project.OrderedTargets);
        }
    }
}