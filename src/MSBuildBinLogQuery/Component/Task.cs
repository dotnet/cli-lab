namespace Microsoft.Build.Logging.Query.Component
{
    public class Task
    {
        public string Name { get; }
        public string Assembly { get; }
        public PropertyManager Parameters { get; }

        public Task(string name, string assembly) : base()
        {
            Name = name;
            Assembly = assembly;
            Parameters = new PropertyManager();
        }
    }
}
