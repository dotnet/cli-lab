using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Task : IHasMessages
    {
        public int Id { get; }
        public string Name { get; }
        public string TaskFile { get; }
        public PropertyManager Parameters { get; }
        public IList<Message> Messages { get; }

        public Task(int id, string name, string taskFile)
        {
            Id = id;
            Name = name;
            TaskFile = taskFile;
            Parameters = new PropertyManager();
            Messages = new List<Message>();
        }
    }
}
