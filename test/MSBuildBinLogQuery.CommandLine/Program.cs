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

            PrintProjectNodes(graphBuilder.Projects.Values.ToList());
        }

        private static void PrintProjectNodes(IEnumerable<ProjectNode> projects)
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

            foreach (var project in projects)
            {
                foreach (var beforeProject in project.Node_BeforeThis.AdjacentNodes)
                {
                    Console.WriteLine($"#{project.Id} -> #{beforeProject.ProjectInfo.Id}");
                }
            }

            Console.WriteLine();

            var projectGraph = new DirectedAcyclicGraph<ProjectNode_BeforeThis>(
                projects.Select(project => project.Node_BeforeThis),
                new ProjectNode_BeforeThis_EqualityComparer());

            var topologicalSortResult = projectGraph.TopologicalSort(out var topologicalOrdering) ? "Success" : "Failed";
            Console.WriteLine($"project topological ordering: {topologicalSortResult}");

            foreach (var project in topologicalOrdering)
            {
                Console.WriteLine($"  #{project.WrappedNode.ProjectInfo.Id}");
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
