using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Construction
{
    public class GraphBuilder
    {
        public Dictionary<int, ProjectNode> Projects { get; }

        private readonly EventArgsDispatcher _eventArgsDispatcher;
        private readonly Mutex _mutex;

        public GraphBuilder()
        {
            Projects = new Dictionary<int, ProjectNode>();

            _eventArgsDispatcher = new EventArgsDispatcher();
            _mutex = new Mutex();

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
            _mutex.WaitOne();

            var id = args.BuildEventContext.ProjectInstanceId;

            if (!Projects.ContainsKey(id))
            {
                var project = new ProjectNode(id, args.ProjectFile, args.TargetNames);

                CopyItems(project, args.Items);
                CopyProperties(project, args.Properties);
                CopyGlobalProperties(project, args.GlobalProperties);

                Projects[id] = project;
            }

            var parent = GetParentNode(args);

            if (parent != null)
            {
                parent.ProjectsDirectlyBeforeThis.Add(Projects[id]);
            }

            _mutex.ReleaseMutex();
        }

        private void TargetStarted(object sender, TargetStartedEventArgs args)
        {
            _mutex.WaitOne();

            var project = Projects[args.BuildEventContext.ProjectInstanceId];
            var target = project.AddOrGetTarget(args.TargetName);

            if (!string.IsNullOrWhiteSpace(args.ParentTarget))
            {
                Console.WriteLine("!!!");
                var parent = project.AddOrGetTarget(args.ParentTarget);

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

            _mutex.ReleaseMutex();
        }

        private ProjectNode GetParentNode(ProjectStartedEventArgs args)
        {
            var parentId = args.ParentProjectBuildEventContext.ProjectInstanceId;
            return Projects.TryGetValue(parentId, out var parent) ? parent : null;
        }

        private void CopyItems(ProjectNode project, IEnumerable items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items.Cast<DictionaryEntry>())
            {
                project.Items.Add(item.Key as string, item.Value as ITaskItem);
            }
        }

        private void CopyProperties(ProjectNode project, IEnumerable properties)
        {
            if (properties == null)
            {
                return;
            }

            foreach (var property in properties.Cast<DictionaryEntry>())
            {
                project.Properties.Set(property.Key as string, property.Value as string);
            }
        }

        private void CopyGlobalProperties(ProjectNode project, IDictionary<string, string> globalProperties)
        {
            if (globalProperties == null)
            {
                return;
            }

            foreach (var globalProperty in globalProperties)
            {
                project.GlobalProperties.Set(globalProperty.Key, globalProperty.Value);
            }
        }
    }
}
