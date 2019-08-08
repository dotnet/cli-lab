using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Project : Component
    {
        public int Id { get; }
        public string ProjectFile { get; }
        public Build ParentBuild { get; }
        public ItemManager Items { get; }
        public PropertyManager Properties { get; }
        public PropertyManager GlobalProperties { get; }
        public ConcurrentDictionary<string, Target> TargetsByName { get; }
        public ConcurrentDictionary<int, Target> TargetsById { get; }
        // TODO: A single project instance can be built concurrently with
        //       distinct EntryPointTargets. Those should be tracked as part of
        //       the overall Project.
        public IReadOnlyList<Target> EntryPointTargets { get; }
        public ConcurrentBag<Target> OrderedTargets { get; }
        public ProjectNode_BeforeThis Node_BeforeThis { get; }

        public Project(int id, ProjectStartedEventArgs args, Build parentBuild) : base()
        {
            Id = id;
            ProjectFile = args.ProjectFile;
            ParentBuild = parentBuild;
            Items = new ItemManager();
            Properties = new PropertyManager();
            GlobalProperties = new PropertyManager();
            TargetsByName = new ConcurrentDictionary<string, Target>();
            TargetsById = new ConcurrentDictionary<int, Target>();
            EntryPointTargets = new List<Target>(
                args.TargetNames
                .Split(';')
                .Where(name => !string.IsNullOrWhiteSpace(name.Trim()))
                .Select(name => GetOrAddTargetWithName(name.Trim())));
            OrderedTargets = new ConcurrentBag<Target>();
            Node_BeforeThis = new ProjectNode_BeforeThis(this);

            CopyItems(args.Items);
            CopyProperties(args.Properties);
            CopyGlobalProperties(args.GlobalProperties);
        }

        public Target GetOrAddTarget(string name, int id)
        {
            var target = new Target(name, id, this);
            TargetsById[id] = target;

            if (TargetsByName.TryAdd(name, target))
            {
                OrderedTargets.Add(target);
                return target;
            }

            return TargetsByName[name];
        }

        private Target GetOrAddTargetWithName(string name)
        {
            return TargetsByName.GetOrAdd(name, new Target(name, null, this));
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
