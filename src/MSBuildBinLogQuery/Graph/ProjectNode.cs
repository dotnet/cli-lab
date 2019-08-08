using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class ProjectNode : IQueryableGraphNode
    {
        public int Id { get; }
        public string ProjectFile { get; }
        public ItemManager Items { get; }
        public PropertyManager Properties { get; }
        public PropertyManager GlobalProperties { get; }
        public ConcurrentDictionary<string, TargetNode> Targets { get; }
        // TODO: A single project instance can be built concurrently with
        //       distinct EntryPointTargets. Those should be tracked as part of
        //       the overall Project.
        public IReadOnlyList<TargetNode> EntryPointTargets { get; }
        public List<TargetNode> OrderedTargets { get; }
        public ConcurrentHashSet<ProjectNode> ProjectsDirectlyBeforeThis { get; }

        public ProjectNode(int id, ProjectStartedEventArgs args) : base()
        {
            Id = id;
            ProjectFile = args.ProjectFile;
            Items = new ItemManager();
            Properties = new PropertyManager();
            GlobalProperties = new PropertyManager();
            Targets = new ConcurrentDictionary<string, TargetNode>();
            EntryPointTargets = new List<TargetNode>(
                args.TargetNames
                .Split(';')
                .Where(name => !string.IsNullOrWhiteSpace(name.Trim()))
                .Select(name => AddOrGetTarget(name.Trim()))
                .ToArray());
            OrderedTargets = new List<TargetNode>();
            ProjectsDirectlyBeforeThis = new ConcurrentHashSet<ProjectNode>();

            CopyItems(args.Items);
            CopyProperties(args.Properties);
            CopyGlobalProperties(args.GlobalProperties);
        }

        public TargetNode AddOrGetOrderedTarget(string name)
        {
            var target = AddOrGetTarget(name);
            OrderedTargets.Add(target);

            return target;
        }

        private TargetNode AddOrGetTarget(string name)
        {
            return Targets.GetOrAdd(name, new TargetNode(name, this));
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
