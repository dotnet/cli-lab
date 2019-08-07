using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Target : IHasMessages
    {
        public string Name { get; }
        public int? Id { get; internal set; }
        public Project Parent { get; }
        public ConcurrentDictionary<int, Task> Tasks { get; }
        public TargetNode_BeforeThis Node_BeforeThis { get; }
        public TargetNode_AfterThis Node_AfterThis { get; }
        public IList<Message> Messages { get; }

        public Target(string name, int? id, Project parent)
        {
            Name = name;
            Id = id;
            Parent = parent;
            Tasks = new ConcurrentDictionary<int, Task>();
            Node_BeforeThis = new TargetNode_BeforeThis(this);
            Node_AfterThis = new TargetNode_AfterThis(this);
            Messages = new List<Message>();
        }

        public Task AddOrGetTask(int id, string name, string taskFile)
        {
            return Tasks.GetOrAdd(id, new Task(id, name, taskFile));
        }
    }
}
