using System.Collections.Concurrent;
using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Target
    {
        public string Name { get; }
        public int? Id { get; internal set; }
        public Project ParentProject { get; }
        public ConcurrentDictionary<int, Task> Tasks { get; }
        public ConcurrentBag<Task> OrderedTasks { get; }
        public TargetNode_BeforeThis Node_BeforeThis { get; }
        public TargetNode_AfterThis Node_AfterThis { get; }

        public Target(string name, int? id, Project parentProject)
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
    }
}
