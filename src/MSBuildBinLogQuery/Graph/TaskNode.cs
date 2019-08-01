using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class TaskNode : IQueryableGraphNode
    {
        public string Name { get; }
        public string Assembly { get; }
        public PropertyManager Parameters { get; }

        public TaskNode(string name, string assembly) : base()
        {
            Name = name;
            Assembly = assembly;
            Parameters = new PropertyManager();
        }
    }
}
