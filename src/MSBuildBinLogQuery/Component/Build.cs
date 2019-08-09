using System.Collections.Concurrent;
using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Build : Component
    {
        public ConcurrentDictionary<int, Project> Projects { get; }

        public Build() : base()
        {
            Projects = new ConcurrentDictionary<int, Project>();
        }

        public Project GetOrAddProject(int id, ProjectStartedEventArgs args)
        {
            return Projects.GetOrAdd(id, new Project(
                id,
                args.ProjectFile,
                args.TargetNames,
                args.Items,
                args.Properties,
                args.GlobalProperties,
                this));
        }
    }
}