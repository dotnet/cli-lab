using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class ProjectNode : IQueryableGraphNode
    {
        public int Id { get; }
        public string ProjectFile { get; }
        public ItemManager Items { get; }
        public PropertyManager Properties { get; }
        public PropertyManager GlobalProperties { get; }
        public List<TargetNode> ExecutedTargets { get; }
        public List<TargetNode> EntryPointTargets { get; }
        public HashSet<ProjectNode> ProjectsBeforeThis { get; }

        public ProjectNode(int id, string projectFile) : base()
        {
            Id = id;
            ProjectFile = projectFile;
            Items = new ItemManager();
            Properties = new PropertyManager();
            GlobalProperties = new PropertyManager();
            ExecutedTargets = new List<TargetNode>();
            EntryPointTargets = new List<TargetNode>();
            ProjectsBeforeThis = new HashSet<ProjectNode>();
        }
    }
}
