using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class ProjectNode_BeforeThis :
        IQueryableGraphNode<ProjectNode_BeforeThis>,
        IShallowCopyableGraphNode<ProjectNode_BeforeThis>
    {
        public ProjectNode ProjectInfo { get; }

        public ISet<ProjectNode_BeforeThis> AdjacentNodes { get; private set; }

        public ProjectNode_BeforeThis(ProjectNode projectInfo)
        {
            ProjectInfo = projectInfo;
            AdjacentNodes = new HashSet<ProjectNode_BeforeThis>();
        }

        public ProjectNode_BeforeThis ShallowCopyAndClearEdges()
        {
            var copy = MemberwiseClone() as ProjectNode_BeforeThis;
            copy.AdjacentNodes = new HashSet<ProjectNode_BeforeThis>();
            return copy;
        }
    }
}
