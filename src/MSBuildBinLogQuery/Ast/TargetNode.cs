using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public class TargetNode : ComponentNode, IEquatable<TargetNode>
    {
        public TargetNode(List<ConstraintNode> constraints = null) : base(null, constraints)
        {
        }

        public TargetNode(AstNode next, List<ConstraintNode> constraints = null) :
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

        public override IEnumerable<IQueryResult> Filter(IEnumerable<Component.Component> components)
        {
            Debug.Assert(components.All(component => component is Project));

            var targets = components
                .Select(component => component as Project)
                .SelectMany(project => project.OrderedTargets);

            return Next?.Filter(targets) ?? targets;
        }
    }
}