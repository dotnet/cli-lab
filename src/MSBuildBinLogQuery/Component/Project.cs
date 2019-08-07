using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Project : BuildComponent
    {
        public int Id { get; }
        public string ProjectFile { get; }
        public ItemManager Items { get; }
        public PropertyManager Properties { get; }
        public PropertyManager GlobalProperties { get; }
        public ConcurrentDictionary<string, Target> TargetsByName { get; }
        public ConcurrentDictionary<int, Target> TargetsById { get; }
        public ImmutableHashSet<Target> EntryPointTargets { get; }
        public ProjectNode_BeforeThis Node_BeforeThis { get; }

        public Project(int id, ProjectStartedEventArgs args) : base()
        {
            Id = id;
            ProjectFile = args.ProjectFile;
            Items = new ItemManager();
            Properties = new PropertyManager();
            GlobalProperties = new PropertyManager();
            TargetsByName = new ConcurrentDictionary<string, Target>();
            TargetsById = new ConcurrentDictionary<int, Target>();
            EntryPointTargets = ImmutableHashSet.Create(
                args.TargetNames
                .Split(';')
                .Where(name => !string.IsNullOrWhiteSpace(name.Trim()))
                .Select(name => AddOrGetTarget(name.Trim()))
                .ToArray()); ;
            Node_BeforeThis = new ProjectNode_BeforeThis(this);

            CopyItems(args.Items);
            CopyProperties(args.Properties);
            CopyGlobalProperties(args.GlobalProperties);
        }

        public Target AddOrGetTarget(string name, int? id = null)
        {
            var target = TargetsByName.GetOrAdd(name, new Target(name, id, this));

            if (id.HasValue)
            {
                target.Id = id;
                TargetsById[id.Value] = target;
            }

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
