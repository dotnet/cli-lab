using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class ProjectNode_BeforeThis :
        IDirectedAcyclicGraphNode<ProjectNode_BeforeThis>,
        IShallowCopyableGraphNode<ProjectNode_BeforeThis>,
        INodeWithComponent<Project>,
        IEquatable<ProjectNode_BeforeThis>
    {
        public Project Component { get; }

        public ISet<ProjectNode_BeforeThis> AdjacentNodes { get; private set; }

        public ProjectNode_BeforeThis(Project projectInfo)
        {
            Component = projectInfo;
            AdjacentNodes = new HashSet<ProjectNode_BeforeThis>();
        }

        public ProjectNode_BeforeThis ShallowCopyAndClearEdges()
        {
            var copy = MemberwiseClone() as ProjectNode_BeforeThis;
            copy.AdjacentNodes = new HashSet<ProjectNode_BeforeThis>();
            return copy;
        }

        public bool Equals([AllowNull] ProjectNode_BeforeThis other)
        {
            return other != null &&
                   EqualityComparer<Project>.Default.Equals(Component, other.Component);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Component);
        }
    }
}