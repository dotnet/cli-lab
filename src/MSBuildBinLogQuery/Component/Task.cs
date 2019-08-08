using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Task
    {
        public int Id { get; }
        public string Name { get; }
        public string TaskFile { get; }
        public Target ParentTarget { get; }
        public PropertyManager Parameters { get; }

        public Task(int id, string name, string taskFile, Target parentTarget)
        {
            Id = id;
            Name = name;
            TaskFile = taskFile;
            ParentTarget = parentTarget;
            Parameters = new PropertyManager();
        }
    }
}
