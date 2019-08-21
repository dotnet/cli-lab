using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Result
{
    public class Build : Component
    {
        public IReadOnlyDictionary<int, Project> ProjectsById => _projectsById;
        public override Component Parent => null;

        private readonly Dictionary<int, Project> _projectsById;

        public Build() : base()
        {
            _projectsById = new Dictionary<int, Project>();
        }

        public Project AddProject(
            int id,
            string projectFile,
            IEnumerable items,
            IEnumerable properties,
            IDictionary<string, string> globalProperties)
        {
            var project = new Project(
                id,
                projectFile,
                items,
                properties,
                globalProperties,
                this);

            if (id != BuildEventContext.InvalidProjectInstanceId)
            {
                _projectsById[id] = project;
            }

            return project;
        }
    }
}