using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<string, TargetNode> Targets { get; }
        public HashSet<TargetNode> EntryPointTargets { get; }
        public HashSet<ProjectNode> ProjectsBeforeThis { get; }

        public ProjectNode(int id, string projectFile, string targetNames) : base()
        {
            Id = id;
            ProjectFile = projectFile;
            Items = new ItemManager();
            Properties = new PropertyManager();
            GlobalProperties = new PropertyManager();
            Targets = new Dictionary<string, TargetNode>();
            EntryPointTargets = new HashSet<TargetNode>(
                targetNames
                .Split(';')
                .Where(name => !string.IsNullOrWhiteSpace(name.Trim()))
                .Select(name => AddOrGetTarget(name.Trim()))); ;
            ProjectsBeforeThis = new HashSet<ProjectNode>();
        }

        public TargetNode AddOrGetTarget(string name)
        {
            if (Targets.ContainsKey(name))
            {
                return Targets[name];
            }

            var target = new TargetNode(name, this);
            Targets[name] = target;

            return target;
        }
    }
}
