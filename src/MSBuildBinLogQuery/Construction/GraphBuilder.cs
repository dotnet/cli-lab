using System.Collections.Concurrent;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Construction
{
    public class GraphBuilder
    {
        public ConcurrentDictionary<int, Project> Projects { get; }

        private readonly EventArgsDispatcher _eventArgsDispatcher;

        public GraphBuilder()
        {
            Projects = new ConcurrentDictionary<int, Project>();
            _eventArgsDispatcher = new EventArgsDispatcher();

            _eventArgsDispatcher.ProjectStarted += ProjectStarted;
            _eventArgsDispatcher.TargetStarted += TargetStarted;
            _eventArgsDispatcher.TaskStarted += TaskStarted;
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
            var project = Projects.GetOrAdd(id, new Project(id, args));
            var parent = GetParentNode(args);

            if (parent != null)
            {
                parent.Node_BeforeThis.AdjacentNodes.Add(project.Node_BeforeThis);
            }
        }

        private void TargetStarted(object sender, TargetStartedEventArgs args)
        {
            var project = Projects[args.BuildEventContext.ProjectInstanceId];
            var target = project.AddOrGetTarget(args.TargetName, args.BuildEventContext.TargetId);

            if (!string.IsNullOrWhiteSpace(args.ParentTarget))
            {
                var parent = project.AddOrGetTarget(args.ParentTarget, args.BuildEventContext.TargetId);

                if (args.BuildReason == TargetBuiltReason.DependsOn)
                {
                    target.Node_BeforeThis.AdjacentNodes.Add(parent.Node_BeforeThis);
                }
                else if (args.BuildReason == TargetBuiltReason.BeforeTargets)
                {
                    parent.Node_BeforeThis.AdjacentNodes.Add(target.Node_BeforeThis);
                }
                else if (args.BuildReason == TargetBuiltReason.AfterTargets)
                {
                    // TODO: args.ParentTarget is empty when args.BuildReason is AfterTargets
                    parent.Node_AfterThis.AdjacentNodes.Add(target.Node_AfterThis);
                }
            }
        }

        private void TaskStarted(object sender, TaskStartedEventArgs args)
        {
            var project = Projects[args.BuildEventContext.ProjectInstanceId];
            var target = project.TargetsById[args.BuildEventContext.TargetId];

            target.AddOrGetTask(args.BuildEventContext.TaskId, args.TaskName, args.TaskFile);
        }

        private Project GetParentNode(ProjectStartedEventArgs args)
        {
            var parentId = args.ParentProjectBuildEventContext.ProjectInstanceId;
            return Projects.TryGetValue(parentId, out var parent) ? parent : null;
        }
    }
}
