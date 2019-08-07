using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Construction
{
    public class GraphBuilder : IHasMessages
    {
        public ConcurrentDictionary<int, Project> Projects { get; }
        public IList<Message> Messages { get; }

        private readonly EventArgsDispatcher _eventArgsDispatcher;

        public GraphBuilder()
        {
            Projects = new ConcurrentDictionary<int, Project>();
            Messages = new List<Message>();
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

        private void MessageRaised(object sender, BuildMessageEventArgs args)
        {
            var message = new Message(args.Message, args.Importance);

            if (args.BuildEventContext.ProjectInstanceId == BuildEventContext.InvalidProjectInstanceId ||
                args.BuildEventContext.ProjectInstanceId == 0)
            {
                Messages.Add(message);
                return;
            }

            var project = Projects[args.BuildEventContext.ProjectInstanceId];

            if (args.BuildEventContext.TargetId == BuildEventContext.InvalidTargetId)
            {
                project.Messages.Add(message);
                return;
            }

            var target = project.TargetsById[args.BuildEventContext.TargetId];

            if (args.BuildEventContext.TaskId == BuildEventContext.InvalidTaskId)
            {
                target.Messages.Add(message);
                return;
            }

            var task = target.Tasks[args.BuildEventContext.TaskId];
            task.Messages.Add(message);
        }

        private Project GetParentNode(ProjectStartedEventArgs args)
        {
            var parentId = args.ParentProjectBuildEventContext.ProjectInstanceId;
            return Projects.TryGetValue(parentId, out var parent) ? parent : null;
        }
    }
}
