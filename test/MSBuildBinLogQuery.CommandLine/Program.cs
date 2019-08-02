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

            var events = new BinaryLogReader(args[0]).ReadEvents();

            var graphBuilder = new GraphBuilder();
            graphBuilder.HandleEvents(events.ToArray());

            PrintProjectNodes(graphBuilder.Projects.Values);
        }

        private static void PrintProjectNodes(IEnumerable<ProjectNode> projects)
        {
            Console.WriteLine("projects:");

            foreach (var project in projects)
            {
                Console.WriteLine($"  project #{project.Id}: {project.ProjectFile}");

                foreach (var target in project.Targets.Values)
                {
                    Console.WriteLine($"    target {target.Name}");
                    Console.WriteLine($"      directly before this: {string.Join(";", target.TargetsDirectlyBeforeThis.Select(beforeThis => beforeThis.Name))}");
                    Console.WriteLine($"      directly after this: {string.Join(";", target.TargetsDirectlyAfterThis.Select(afterThis => afterThis.Name))}");
                }
            }

            Console.WriteLine();

            foreach (var project in projects)
            {
                foreach (var beforeProject in project.ProjectsDirectlyBeforeThis)
                {
                    Console.WriteLine($"#{project.Id} -> #{beforeProject.Id}");
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
