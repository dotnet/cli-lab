using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
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
        public HashSet<ProjectNode> ProjectsDirectlyBeforeThis { get; }

        public ProjectNode(int id, ProjectStartedEventArgs args) : base()
        {
            Id = id;
            ProjectFile = args.ProjectFile;
            Items = new ItemManager();
            Properties = new PropertyManager();
            GlobalProperties = new PropertyManager();
            Targets = new Dictionary<string, TargetNode>();
            EntryPointTargets = new HashSet<TargetNode>(
                args.TargetNames
                .Split(';')
                .Where(name => !string.IsNullOrWhiteSpace(name.Trim()))
                .Select(name => AddOrGetTarget(name.Trim()))); ;
            ProjectsDirectlyBeforeThis = new HashSet<ProjectNode>();

            CopyItems(args.Items);
            CopyProperties(args.Properties);
            CopyGlobalProperties(args.GlobalProperties);
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

        private void CopyItems(IEnumerable items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items.Cast<DictionaryEntry>())
            {
                Items.Add(item.Key as string, item.Value as ITaskItem);
            }
        }

        private void CopyProperties(IEnumerable properties)
        {
            if (properties == null)
            {
                return;
            }

            foreach (var property in properties.Cast<DictionaryEntry>())
            {
                Properties.Set(property.Key as string, property.Value as string);
            }
        }

        private void CopyGlobalProperties(IDictionary<string, string> globalProperties)
        {
            if (globalProperties == null)
            {
                return;
            }

            foreach (var globalProperty in globalProperties)
            {
                GlobalProperties.Set(globalProperty.Key, globalProperty.Value);
            }
        }
    }
}
