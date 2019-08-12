using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Project : Component, IEquatable<Project>
    {
        public int Id { get; }
        public string ProjectFile { get; }
        public Build ParentBuild { get; }
        public ItemManager Items { get; }
        public PropertyManager Properties { get; }
        public PropertyManager GlobalProperties { get; }
        public ProjectNode_BeforeThis Node_BeforeThis { get; }

        public IReadOnlyDictionary<int, Target> TargetsById => _targetsById;
        public IReadOnlyDictionary<string, Target> TargetsByName => _targetsByName;
        public IReadOnlyList<Target> OrderedTargets => _orderedTargets;
        // TODO: A single project instance can be built concurrently with
        //       distinct EntryPointTargets. Those should be tracked as part of
        //       the overall Project.
        public IReadOnlyList<Target> EntryPointTargets => _entryPointTargets;
        public override Component Parent => ParentBuild;

        private readonly Dictionary<int, Target> _targetsById;
        private readonly Dictionary<string, Target> _targetsByName;
        private readonly List<Target> _orderedTargets;
        private readonly List<Target> _entryPointTargets;

        public Project(
            int id,
            string projectFile,
            IEnumerable items,
            IEnumerable properties,
            IDictionary<string, string> globalProperties,
            Build parentBuild) : base()
        {
            Id = id;
            ProjectFile = projectFile;
            ParentBuild = parentBuild;
            Items = new ItemManager();
            Properties = new PropertyManager();
            GlobalProperties = new PropertyManager();
            Node_BeforeThis = new ProjectNode_BeforeThis(this);

            _targetsById = new Dictionary<int, Target>();
            _targetsByName = new Dictionary<string, Target>();
            _orderedTargets = new List<Target>();
            _entryPointTargets = new List<Target>();

            CopyItems(items);
            CopyProperties(properties);
            CopyGlobalProperties(globalProperties);
        }

        public Target AddTarget(int id, string name, bool isEntryPointTarget)
        {
            var target = new Target(id, name, this);

            _targetsByName[name] = target;
            _orderedTargets.Add(target);

            if (id != BuildEventContext.InvalidTargetId)
            {
                _targetsById[id] = target;
            }

            if (isEntryPointTarget)
            {
                _entryPointTargets.Add(target);
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

        public bool Equals([AllowNull] Project other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
