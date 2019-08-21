using System.Linq;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Construction
{
    public class GraphBuilder
    {
        private readonly EventArgsDispatcher _eventArgsDispatcher;
        private readonly List<ProjectStartedEventArgs> _projectArgs;
        private readonly Dictionary<int, List<TargetStartedEventArgs>> _targetArgs;
        private readonly Dictionary<(int projectId, int targetId), List<TaskStartedEventArgs>> _taskArgs;
        private readonly Dictionary<int, Dictionary<string, Target>> _targetsByName;
        private readonly List<BuildMessageEventArgs> _messageArgs;
        private readonly List<BuildWarningEventArgs> _warningArgs;
        private readonly List<BuildErrorEventArgs> _errorArgs;

        public GraphBuilder()
        {
            _eventArgsDispatcher = new EventArgsDispatcher();
            _projectArgs = new List<ProjectStartedEventArgs>();
            _targetArgs = new Dictionary<int, List<TargetStartedEventArgs>>();
            _targetsByName = new Dictionary<int, Dictionary<string, Target>>();
            _taskArgs = new Dictionary<(int projectId, int targetId), List<TaskStartedEventArgs>>();
            _messageArgs = new List<BuildMessageEventArgs>();
            _warningArgs = new List<BuildWarningEventArgs>();
            _errorArgs = new List<BuildErrorEventArgs>();

            _eventArgsDispatcher.ProjectStarted += ProjectStarted;
            _eventArgsDispatcher.TargetStarted += TargetStarted;
            _eventArgsDispatcher.TaskStarted += TaskStarted;
            _eventArgsDispatcher.MessageRaised += MessageRaised;
            _eventArgsDispatcher.WarningRaised += WarningRaised;
            _eventArgsDispatcher.ErrorRaised += ErrorRaised;
        }

        public Result.Build HandleEvents(params BuildEventArgs[] buildEvents)
        {
            foreach (var buildEvent in buildEvents)
            {
                _eventArgsDispatcher.Dispatch(buildEvent);
            }

            var build = new Result.Build();

            AddProjects(build);
            AddLogs(build);

            return build;
        }

        private void ProjectStarted(object sender, ProjectStartedEventArgs args)
        {
            var id = args.BuildEventContext.ProjectInstanceId;

            _projectArgs.Add(args);

            if (!_targetArgs.ContainsKey(id))
            {
                _targetArgs[id] = new List<TargetStartedEventArgs>();
            }

            if (!_targetsByName.ContainsKey(id))
            {
                _targetsByName[id] = new Dictionary<string, Target>();
            }
        }

        private void TargetStarted(object sender, TargetStartedEventArgs args)
        {
            var id = args.BuildEventContext.TargetId;
            var projectId = args.BuildEventContext.ProjectInstanceId;

            _targetArgs[projectId].Add(args);
            _taskArgs[(projectId, targetId: id)] = new List<TaskStartedEventArgs>();
        }

        private void TaskStarted(object sender, TaskStartedEventArgs args)
        {
            var projectId = args.BuildEventContext.ProjectInstanceId;
            var targetId = args.BuildEventContext.TargetId;

            _taskArgs[(projectId, targetId)].Add(args);
        }

        private void MessageRaised(object sender, BuildMessageEventArgs args)
        {
            _messageArgs.Add(args);
        }

        private void WarningRaised(object sender, BuildWarningEventArgs args)
        {
            _warningArgs.Add(args);
        }

        private void ErrorRaised(object sender, BuildErrorEventArgs args)
        {
            _errorArgs.Add(args);
        }

        private void AddProjects(Result.Build build)
        {
            var projectArgsById = new Dictionary<int, ProjectStartedEventArgs>();

            foreach (var args in _projectArgs)
            {
                var id = args.BuildEventContext.ProjectInstanceId;
                var targetsByName = _targetsByName[id];

                projectArgsById[id] = args;

                var project = build.AddProject(
                    id,
                    args.ProjectFile,
                    args.Items,
                    args.Properties,
                    args.GlobalProperties);

                var entryPointTargetNames = args.TargetNames
                    .Split(';')
                    .Where(name => !string.IsNullOrWhiteSpace(name.Trim()))
                    .ToHashSet();
                
                AddTargets(project, id, entryPointTargetNames);
            }

            foreach (var pair in build.ProjectsById)
            {
                var id = pair.Key;
                var project = pair.Value;
                var args = projectArgsById[id];
                var parentId = args.ParentProjectBuildEventContext.ProjectInstanceId;

                if (build.ProjectsById.TryGetValue(parentId, out var parent))
                {
                    parent.Node_BeforeThis.AdjacentNodes.Add(project.Node_BeforeThis);
                }
            }
        }

        private void AddTargets(Project project, int projectId, HashSet<string> entryPointTargetNames)
        {
            var targetArgs = _targetArgs[projectId];
            var targetArgsById = new Dictionary<int, TargetStartedEventArgs>();

            foreach (var args in targetArgs)
            {
                var targetId = args.BuildEventContext.TargetId;
                var name = args.TargetName;

                var target = project.AddTarget(targetId, name, entryPointTargetNames.Contains(name));

                targetArgsById[targetId] = args;

                AddTasks(target, projectId, targetId);
            }

            foreach (var pair in project.TargetsById)
            {
                var id = pair.Key;
                var target = pair.Value;
                var args = targetArgsById[id];

                if (!string.IsNullOrWhiteSpace(args.ParentTarget))
                {
                    var parent =
                        project.TargetsByName.ContainsKey(args.ParentTarget) ?
                        project.TargetsByName[args.ParentTarget] :
                        project.AddTarget(BuildEventContext.InvalidTargetId, args.ParentTarget, entryPointTargetNames.Contains(args.ParentTarget));

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
        }

        private void AddTasks(Target target, int projectId, int targetId)
        {
            var taskArgs = _taskArgs[(projectId, targetId)];

            foreach (var args in taskArgs)
            {
                var taskId = args.BuildEventContext.TaskId;
                target.AddTask(taskId, args.TaskName, args.TaskFile);
            }
        }

        private void AddLogs(Result.Build build)
        {
            foreach (var args in _messageArgs)
            {
                var parent = GetParentComponent(args, build);
                var message = new Message(args.Message, parent, args.Importance);
                parent.AddMessage(message);
            }

            foreach (var args in _warningArgs)
            {
                var parent = GetParentComponent(args, build);
                var warning = new Warning(args.Message, parent);
                parent.AddWarning(warning);
            }

            foreach (var args in _errorArgs)
            {
                var parent = GetParentComponent(args, build);
                var error = new Error(args.Message, parent);
                parent.AddError(error);
            }
        }

        private Result.Component GetParentComponent(BuildEventArgs args, Result.Build build)
        {
            return GetParentComponent(
                args.BuildEventContext.ProjectInstanceId,
                args.BuildEventContext.TargetId,
                args.BuildEventContext.TaskId,
                build);
        }

        private Result.Component GetParentComponent(int projectId, int targetId, int taskId, Result.Build build)
        {
            if (projectId == BuildEventContext.InvalidProjectInstanceId || projectId == 0)
            {
                return build;
            }

            var project = build.ProjectsById[projectId];

            if (targetId == BuildEventContext.InvalidTargetId)
            {
                return project;
            }

            var target = project.TargetsById[targetId];

            if (taskId == BuildEventContext.InvalidTaskId)
            {
                return target;
            }

            return target.TasksById[taskId];
        }
    }
}
