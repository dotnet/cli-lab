using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Construction
{
    public class GraphBuilder
    {
        public BuildResult Build { get; }

        private readonly EventArgsDispatcher _eventArgsDispatcher;

        public GraphBuilder()
        {
            Build = new BuildResult();
            _eventArgsDispatcher = new EventArgsDispatcher();

            _eventArgsDispatcher.ProjectStarted += ProjectStarted;
            _eventArgsDispatcher.TargetStarted += TargetStarted;
            _eventArgsDispatcher.TaskStarted += TaskStarted;
            _eventArgsDispatcher.MessageRaised += MessageRaised;
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
            var project = Build.Projects.GetOrAdd(id, new Project(id, args));
            var parent = GetParentProject(args);

            if (parent != null)
            {
                parent.Node_BeforeThis.AdjacentNodes.Add(project.Node_BeforeThis);
            }
        }

        private void TargetStarted(object sender, TargetStartedEventArgs args)
        {
            var project = Build.Projects[args.BuildEventContext.ProjectInstanceId];
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
            var project = Build.Projects[args.BuildEventContext.ProjectInstanceId];
            var target = project.TargetsById[args.BuildEventContext.TargetId];

            target.AddOrGetTask(args.BuildEventContext.TaskId, args.TaskName, args.TaskFile);
        }

        private void MessageRaised(object sender, BuildMessageEventArgs args)
        {
            var message = new Message(args.Message, args.Importance);
            var containingComponent = GetContainingComponent(
                args.BuildEventContext.ProjectInstanceId,
                args.BuildEventContext.TargetId,
                args.BuildEventContext.TaskId);

            containingComponent.Messages.Add(message);
        }

        private Project GetParentProject(ProjectStartedEventArgs args)
        {
            var parentId = args.ParentProjectBuildEventContext.ProjectInstanceId;
            return Build.Projects.TryGetValue(parentId, out var parent) ? parent : null;
        }

        private BuildComponent GetContainingComponent(int projectInstanceId, int targetId, int taskId)
        {
            if (projectInstanceId == BuildEventContext.InvalidProjectInstanceId || projectInstanceId == 0)
            {
                return Build;
            }

            var project = Build.Projects[projectInstanceId];

            if (targetId == BuildEventContext.InvalidTargetId)
            {
                return project;
            }

            var target = project.TargetsById[targetId];

            if (taskId == BuildEventContext.InvalidTaskId)
            {
                return target;
            }

            return target.Tasks[taskId];
        }
    }
}
