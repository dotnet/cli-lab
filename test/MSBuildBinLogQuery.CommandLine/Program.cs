using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Logging.Query.Component;
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

            PrintProjectNodes(graphBuilder.Projects.Values);
        }

        private static void PrintProjectNodes(IEnumerable<Project> projects)
        {
            Console.WriteLine("projects:");

            foreach (var project in projects)
            {
                Console.WriteLine($"  project #{project.Id}: {project.ProjectFile}");

                // foreach (var target in project.Targets.Values)
                // {
                //     Console.WriteLine($"    target {target.Name}");
                //     Console.WriteLine($"      directly before this: {string.Join(";", target.Node_BeforeThis.AdjacentNodes.Select(beforeThis => beforeThis.Name))}");
                //     Console.WriteLine($"      directly after this: {string.Join(";", target.Node_AfterThis.AdjacentNodes.Select(afterThis => afterThis.Name))}");
                // }
            }

            Console.WriteLine();

            Console.WriteLine("project graph edges:");

            foreach (var project in projects)
            {
                foreach (var beforeProject in project.Node_BeforeThis.AdjacentNodes)
                {
                    Console.WriteLine($"  #{project.Id} -> #{beforeProject.ProjectInfo.Id}");
                }
            }

            Console.WriteLine();

            var projectGraph = new DirectedAcyclicGraph<ProjectNode_BeforeThis>(projects.Select(project => project.Node_BeforeThis));

            var topologicalSortResult = projectGraph.TopologicalSort(out var topologicalOrdering) ? "Success" : "Failed";
            Console.WriteLine($"project topological ordering: {topologicalSortResult}");

            foreach (var project in topologicalOrdering)
            {
                Console.WriteLine($"  #{project.ProjectInfo.Id}");
            }

            Console.WriteLine();

            var reachableCalculationResult = projectGraph.GetReachableNodes(out var reachables) ? "Success" : "Failed";
            Console.WriteLine($"reachable from each project: {reachableCalculationResult}");

            foreach (var pair in reachables)
            {
                var reachableNodes = string.Join(", ", pair.Value.Select(node => $"#{node.ProjectInfo.Id}"));
                Console.WriteLine($"  #{pair.Key.ProjectInfo.Id}: {reachableNodes}");
            }

            Console.WriteLine();

            var reversedGraph = projectGraph.Reverse();

            Console.WriteLine("reversed project graph:");

            foreach (var node in reversedGraph.Nodes)
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
    }
}
