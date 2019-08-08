using System.Collections.Concurrent;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Construction
{
    public class GraphBuilder
    {
        public ConcurrentDictionary<int, ProjectNode> Projects { get; }

        private readonly EventArgsDispatcher _eventArgsDispatcher;

        public GraphBuilder()
        {
            Projects = new ConcurrentDictionary<int, ProjectNode>();
            _eventArgsDispatcher = new EventArgsDispatcher();

            _eventArgsDispatcher.ProjectStarted += ProjectStarted;
            _eventArgsDispatcher.TargetStarted += TargetStarted;
        }

        public void HandleEvents(params BuildEventArgs[] buildEvents)
        {
            foreach (var buildEvent in buildEvents)
            {
                _eventArgsDispatcher.Dispatch(buildEvent);
            }
        }

        private void ProjectStarted(object sender, ProjectStartedEventArgs args)
        {
            var id = args.BuildEventContext.ProjectInstanceId;
            Projects.TryAdd(id, new ProjectNode(id, args));

            var parent = GetParentNode(args);

            if (parent != null)
            {
                parent.ProjectsDirectlyBeforeThis.Add(Projects[id]);
            }
        }

        private void TargetStarted(object sender, TargetStartedEventArgs args)
        {
            var project = Projects[args.BuildEventContext.ProjectInstanceId];
            var target = project.AddOrGetOrderedTarget(args.TargetName);

            if (!string.IsNullOrWhiteSpace(args.ParentTarget))
            {
                var parent = project.AddOrGetOrderedTarget(args.ParentTarget);

                if (args.BuildReason == TargetBuiltReason.DependsOn)
                {
                    target.TargetsDirectlyBeforeThis.Add(parent);
                }
                else if (args.BuildReason == TargetBuiltReason.BeforeTargets)
                {
                    parent.TargetsDirectlyBeforeThis.Add(target);
                }
                else if (args.BuildReason == TargetBuiltReason.AfterTargets)
                {
                    // TODO: args.ParentTarget is empty when args.BuildReason is AfterTargets
                    parent.TargetsDirectlyAfterThis.Add(target);
                }
            }
        }

        private ProjectNode GetParentNode(ProjectStartedEventArgs args)
        {
            var parentId = args.ParentProjectBuildEventContext.ProjectInstanceId;
            return Projects.TryGetValue(parentId, out var parent) ? parent : null;
        }
    }
}
