using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Construction
{
    public class GraphBuilder
    {
        private readonly EventArgsDispatcher _eventArgsDispatcher;
        private readonly List<ProjectStartedEventArgs> _projectArgs;
        private readonly Dictionary<int, List<TargetStartedEventArgs>> _targetArgs;
        private readonly Dictionary<(int projectId, int targetId), List<TaskStartedEventArgs>> _taskArgs;
        private readonly Dictionary<int, Dictionary<string, Target>> _targetsByName;

        public GraphBuilder()
        {
            _eventArgsDispatcher = new EventArgsDispatcher();
            _projectArgs = new List<ProjectStartedEventArgs>();
            _targetArgs = new Dictionary<int, List<TargetStartedEventArgs>>();
            _targetsByName = new Dictionary<int, Dictionary<string, Target>>();
            _taskArgs = new Dictionary<(int projectId, int targetId), List<TaskStartedEventArgs>>();

            _eventArgsDispatcher.ProjectStarted += ProjectStarted;
            _eventArgsDispatcher.TargetStarted += TargetStarted;
            _eventArgsDispatcher.TaskStarted += TaskStarted;
        }

        public Component.Build HandleEvents(params BuildEventArgs[] buildEvents)
        {
            foreach (var buildEvent in buildEvents)
            {
                _eventArgsDispatcher.Dispatch(buildEvent);
            }

            var build = new Component.Build();
            AddProjects(build);
            return build;
        }

        private void ProjectStarted(object sender, ProjectStartedEventArgs args)
        {
            var id = args.BuildEventContext.ProjectInstanceId;

            _projectArgs.Add(args);
            _targetArgs[id] = new List<TargetStartedEventArgs>();
            _targetsByName[id] = new Dictionary<string, Target>();
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

        private void AddProjects(Component.Build build)
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
    }
}
