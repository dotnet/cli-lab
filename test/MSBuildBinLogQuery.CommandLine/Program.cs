using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Construction;
using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Messaging;

namespace Microsoft.Build.Logging.Query.Commandline
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                PrintErrorMessage("Exactly one argument is required");
            }

            if (!File.Exists(args[0]))
            {
                PrintErrorMessage($"File not found: {args[0]}");
            }

            using var binaryLogReader = new BinaryLogReader(args[0]);
            var events = binaryLogReader.ReadEvents();

            var graphBuilder = new GraphBuilder();
            graphBuilder.HandleEvents(events.ToArray());

            PrintProjectNodes(graphBuilder.Build);
        }

        private static void PrintProjectNodes(BuildResult build)
        {
            PrintBuild(build, printWarnings: true, printErrors: true);

            var projectGraph = new DirectedAcyclicGraph<ProjectNode_BeforeThis>(
                build.Projects.Values.Select(project => project.Node_BeforeThis),
                new ProjectNode_BeforeThis_EqualityComparer());

            PrintProjectGraph(projectGraph);
            PrintProjectTopologicalOrdering(projectGraph);
            PrintReachableProjects(projectGraph);
            PrintReversedProjectGraph(projectGraph);

            // PrintAllMessages(build);
            PrintAllWarnings(build);
            PrintAllErrors(build);
        }

        private static void PrintErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            Environment.Exit(1);
        }

        private static void PrintProjectGraph(DirectedAcyclicGraph<ProjectNode_BeforeThis> graph, string header = "project graph:")
        {
            Console.WriteLine(header);

            foreach (var node in graph.Nodes.Keys)
            {
                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    Console.WriteLine($"  #{node.ProjectInfo.Id} -> #{adjacentNode.ProjectInfo.Id}");
                }
            }

            Console.WriteLine();
        }

        private static void PrintReversedProjectGraph(DirectedAcyclicGraph<ProjectNode_BeforeThis> graph, string header = "reversed project graph:")
        {
            PrintProjectGraph(graph.Reverse(), header);
        }

        private static void PrintProjectTopologicalOrdering(DirectedAcyclicGraph<ProjectNode_BeforeThis> graph, string header = "project topological ordering:")
        {
            var topologicalSortResult = graph.TopologicalSort(out var topologicalOrdering) ? "Success" : "Failed";
            Console.WriteLine($"{header}: {topologicalSortResult}");

            foreach (var project in topologicalOrdering)
            {
                Console.WriteLine($"  #{project.WrappedNode.ProjectInfo.Id}");
            }

            Console.WriteLine();
        }

        private static void PrintReachableProjects(DirectedAcyclicGraph<ProjectNode_BeforeThis> graph, string header = "reachable projects:")
        {
            var reachableCalculationResult = graph.CalculateReachableNodes() ? "Success" : "Failed";
            Console.WriteLine($"{header}: {reachableCalculationResult}");

            foreach (var node in graph.Nodes.Values)
            {
                var reachableNodes = string.Join(", ", node.ReachableFromThis.Select(node => $"#{node.WrappedNode.ProjectInfo.Id}"));
                Console.WriteLine($"  #{node.WrappedNode.ProjectInfo.Id}: {reachableNodes}");
            }

            Console.WriteLine();
        }

        private static void PrintBuild(
            BuildResult build,
            string header = "build:",
            bool printTargetGraph = false,
            bool printMessages = false,
            bool printWarnings = false,
            bool printErrors = false)
        {
            Console.WriteLine(header);

            foreach (var project in build.Projects.Values)
            {
                Console.WriteLine($"  project #{project.Id}: {project.ProjectFile}");

                foreach (var target in project.TargetsByName.Values)
                {
                    Console.WriteLine($"    target #{target.Id} {target.Name}");

                    if (printTargetGraph)
                    {
                        Console.WriteLine($"      directly before this: {string.Join(";", target.Node_BeforeThis.AdjacentNodes.Select(beforeThis => beforeThis.TargetInfo.Name))}");
                        Console.WriteLine($"      directly after this: {string.Join(";", target.Node_AfterThis.AdjacentNodes.Select(afterThis => afterThis.TargetInfo.Name))}");
                    }

                    foreach (var task in target.Tasks.Values)
                    {
                        Console.WriteLine($"      task #{task.Id} {task.Name}");
                        PrintComponentLogs(task, printMessages, printWarnings, printErrors, "        ");
                    }

                    PrintComponentLogs(target, printMessages, printWarnings, printErrors, "      ");
                }

                PrintComponentLogs(project, printMessages, printWarnings, printErrors, "    ");
            }

            PrintComponentLogs(build, printMessages, printWarnings, printErrors, "  ");

            Console.WriteLine();
        }

        private static void PrintAllMessages(BuildResult build, string header = "messages:")
        {
            Console.WriteLine(header);

            foreach (var message in build.AllMessages)
            {
                Console.WriteLine($"  importance {message.Importance}: {message.Text}");
            }

            Console.WriteLine();
        }

        private static void PrintAllWarnings(BuildResult build, string header = "warnings:")
        {
            Console.WriteLine(header);

            foreach (var warning in build.AllWarnings)
            {
                Console.WriteLine($"  {warning.Text}");
            }

            Console.WriteLine();
        }

        private static void PrintAllErrors(BuildResult build, string header = "errors:")
        {
            Console.WriteLine(header);

            foreach (var error in build.AllErrors)
            {
                Console.WriteLine($"  {error.Text}");
            }

            Console.WriteLine();
        }

        private static void PrintComponentLogs(
            BuildComponent component,
            bool printMessages,
            bool printWarnings,
            bool printErrors,
            string indent)
        {
            PrintLogs(component.Messages, printMessages, indent);
            PrintLogs(component.Warnings, printWarnings, indent);
            PrintLogs(component.Errors, printErrors, indent);
        }

        private static void PrintLogs(IEnumerable<Log> logs, bool flag, string indent)
        {
            if (flag)
            {
                foreach (var log in logs)
                {
                    Console.WriteLine($"{indent}{log.GetType().Name}: {log.Text}");
                }
            }
        }

        private class ProjectNode_BeforeThis_EqualityComparer : IEqualityComparer<ProjectNode_BeforeThis>
        {
            public bool Equals(ProjectNode_BeforeThis x, ProjectNode_BeforeThis y)
            {
                return x.ProjectInfo.Id == y.ProjectInfo.Id;
            }

            public int GetHashCode(ProjectNode_BeforeThis obj)
            {
                return obj.ProjectInfo.Id;
            }
        }
    }
}
