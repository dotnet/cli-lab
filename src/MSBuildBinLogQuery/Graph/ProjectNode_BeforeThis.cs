using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class ProjectNode_BeforeThis : IQueryableGraphNode<ProjectNode_BeforeThis>
    {
        public ProjectNode ProjectInfo { get; }
        public ConcurrentHashSet<ProjectNode_BeforeThis> AdjacentNodes { get; }

        public ProjectNode_BeforeThis(ProjectNode projectInfo)
        {
            ProjectInfo = projectInfo;
            AdjacentNodes = new ConcurrentHashSet<ProjectNode_BeforeThis>();
        }
    }
}
