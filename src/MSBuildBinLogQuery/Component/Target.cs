using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Target : Component, IEquatable<Target>, IComponentWithId
    {
        public int Id { get; }
        public string Name { get; }
        public Project ParentProject { get; }
        public TargetNode_BeforeThis Node_BeforeThis { get; }
        public TargetNode_AfterThis Node_AfterThis { get; }
        public override Component Parent => ParentProject;

        public IReadOnlyDictionary<int, Task> TasksById => _tasksById;
        public IReadOnlyList<Task> OrderedTasks => _orderedTasks;

        private readonly Dictionary<int, Task> _tasksById;
        private readonly List<Task> _orderedTasks;

        public Target(int id, string name, Project parentProject) : base()
        {
            Id = id;
            Name = name;
            ParentProject = parentProject;
            Node_BeforeThis = new TargetNode_BeforeThis(this);
            Node_AfterThis = new TargetNode_AfterThis(this);

            _tasksById = new Dictionary<int, Task>();
            _orderedTasks = new List<Task>();
        }

        public Task AddTask(int id, string name, string taskFile)
        {
            var task = new Task(id, name, taskFile, this);

            _orderedTasks.Add(task);

            if (id != BuildEventContext.InvalidTaskId)
            {
                _tasksById[id] = task;
            }

            return task;
        }

        public bool Equals([AllowNull] Target other)
        {
            return other != null &&
                   Id == other.Id &&
                   EqualityComparer<Project>.Default.Equals(ParentProject, other.ParentProject);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ParentProject);
        }
    }
}
