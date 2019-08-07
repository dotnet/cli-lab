namespace Microsoft.Build.Logging.Query.Component
{
    public class Task
    {
        public int Id { get; }
        public string Name { get; }
        public string TaskFile { get; }
        public PropertyManager Parameters { get; }

        public Task(int id, string name, string taskFile)
        {
            Id = id;
            Name = name;
            TaskFile = taskFile;
            Parameters = new PropertyManager();
        }
    }
}
