using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Target : Component, IEquatable<Target>
    {
        public string Name { get; }
        public int? Id { get; internal set; }
        public Project ParentProject { get; }
        public ConcurrentDictionary<int, Task> Tasks { get; }
        public ConcurrentBag<Task> OrderedTasks { get; }
        public TargetNode_BeforeThis Node_BeforeThis { get; }
        public TargetNode_AfterThis Node_AfterThis { get; }

        public Target(string name, int? id, Project parentProject) : base()
        {
            Name = name;
            Id = id;
            ParentProject = parentProject;
            Tasks = new ConcurrentDictionary<int, Task>();
            OrderedTasks = new ConcurrentBag<Task>();
            Node_BeforeThis = new TargetNode_BeforeThis(this);
            Node_AfterThis = new TargetNode_AfterThis(this);
        }

        public Task GetOrAddTask(int id, string name, string taskFile)
        {
            var task = new Task(id, name, taskFile, this);
            Tasks[id] = task;

            if (Tasks.TryAdd(id, task))
            {
                OrderedTasks.Add(task);
                return task;
            }

            return Tasks[id];
        }

        public bool Equals([AllowNull] Target other)
        {
            return other != null &&
                   Name == other.Name &&
                   EqualityComparer<Project>.Default.Equals(ParentProject, other.ParentProject);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ParentProject);
        }
    }
}
