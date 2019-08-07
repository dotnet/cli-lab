using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Logging.Query.Construction;
using Microsoft.Build.Logging.Query.Graph;

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

        private static void PrintProjectNodes(Component.BuildResult build)
        {
            Console.WriteLine("build:");

            foreach (var project in build.Projects.Values)
            {
                Console.WriteLine($"  project #{project.Id}: {project.ProjectFile}");

                // foreach (var target in project.Targets.Values)
                // {
                //     Console.WriteLine($"    target {target.Name}");
                //     Console.WriteLine($"      directly before this: {string.Join(";", target.Node_BeforeThis.AdjacentNodes.Select(beforeThis => beforeThis.Name))}");
                //     Console.WriteLine($"      directly after this: {string.Join(";", target.Node_AfterThis.AdjacentNodes.Select(afterThis => afterThis.Name))}");
                // }

                foreach (var target in project.TargetsByName.Values)
                {
                    Console.WriteLine($"    target {target.Name}");

                    foreach (var task in target.Tasks.Values)
                    {
                        Console.WriteLine($"      task #{task.Id} {task.Name}");

                        foreach (var message in task.Messages)
                        {
                            Console.WriteLine($"        message: {message.Text}");
                        }
                    }

                    foreach (var message in target.Messages)
                    {
                        Console.WriteLine($"      message: {message.Text}");
                    }
                }

                foreach (var message in project.Messages)
                {
                    Console.WriteLine($"    message: {message.Text}");
                }
            }

            foreach (var message in build.Messages)
            {
                Console.WriteLine($"message: {message.Text}");
            }

            Console.WriteLine();

            Console.WriteLine("project graph edges:");

            foreach (var project in build.Projects.Values)
            {
                foreach (var beforeProject in project.Node_BeforeThis.AdjacentNodes)
                {
                    Console.WriteLine($"  #{project.Id} -> #{beforeProject.ProjectInfo.Id}");
                }
            }

            Console.WriteLine();

            var projectGraph = new DirectedAcyclicGraph<ProjectNode_BeforeThis>(
                build.Projects.Values.Select(project => project.Node_BeforeThis),
                new ProjectNode_BeforeThis_EqualityComparer());

            var topologicalSortResult = projectGraph.TopologicalSort(out var topologicalOrdering) ? "Success" : "Failed";
            Console.WriteLine($"project topological ordering: {topologicalSortResult}");

            foreach (var project in topologicalOrdering)
            {
                Console.WriteLine($"  #{project.WrappedNode.ProjectInfo.Id}");
            }

            Console.WriteLine();

            var reachableCalculationResult = projectGraph.CalculateReachableNodes() ? "Success" : "Failed";
            Console.WriteLine($"reachable from each project: {reachableCalculationResult}");

            foreach (var node in projectGraph.Nodes.Values)
            {
                var reachableNodes = string.Join(", ", node.ReachableFromThis.Select(node => $"#{node.WrappedNode.ProjectInfo.Id}"));
                Console.WriteLine($"  #{node.WrappedNode.ProjectInfo.Id}: {reachableNodes}");
            }

            Console.WriteLine();

            var reversedGraph = projectGraph.Reverse();

            Console.WriteLine("reversed project graph:");

            foreach (var node in reversedGraph.Nodes.Keys)
            {
                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    Console.WriteLine($"  #{node.ProjectInfo.Id} -> #{adjacentNode.ProjectInfo.Id}");
                }
            }
        }

        private static void PrintErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            Environment.Exit(1);
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
