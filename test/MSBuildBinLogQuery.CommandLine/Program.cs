using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Logging.Query.Ast;
using Microsoft.Build.Logging.Query.Component;
using Microsoft.Build.Logging.Query.Construction;
using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Messaging;
using Microsoft.Build.Logging.Query.Parse;
using Microsoft.Build.Logging.Query.Scan;
using Microsoft.Build.Logging.Query.Token;

namespace Microsoft.Build.Logging.Query.Commandline
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
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
            var build = graphBuilder.HandleEvents(events.ToArray());

            PrintProjectNodes(build);
            PrintScanAndParseResults(args[1]);
            PrintInterpResults(build, args[1]);
        }

        private static void PrintProjectNodes(Component.Build build)
        {
            PrintBuild(build, printErrors: true, printWarnings: true);

            var projectGraph = new DirectedAcyclicGraph<ProjectNode_BeforeThis>(build.ProjectsById.Values.Select(project => project.Node_BeforeThis));

            PrintProjectGraph(projectGraph);
            PrintProjectTopologicalOrdering(projectGraph);
            PrintReachableProjects(projectGraph);
            PrintReversedProjectGraph(projectGraph);

            // PrintAllLogs(build.AllMessages, "all messages:");
            PrintAllLogs(build.AllWarnings, "all warnings:");
            PrintAllLogs(build.AllErrors, "all errors:");
        }

        private static void PrintScanAndParseResults(string expression)
        {
            PrintTokens(expression);
            PrintAst(expression);
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

            Console.WriteLine("  graph TD");

            foreach (var node in graph.Nodes)
            {
                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    Console.WriteLine($"  {node.ProjectInfo.Id} --> {adjacentNode.ProjectInfo.Id}");
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
                Console.WriteLine($"  #{project.ProjectInfo.Id}");
            }

            Console.WriteLine();
        }

        private static void PrintReachableProjects(DirectedAcyclicGraph<ProjectNode_BeforeThis> graph, string header = "reachable projects:")
        {
            var reachableCalculationResult = graph.GetReachableNodes(out var reachables) ? "Success" : "Failed";
            Console.WriteLine($"{header}: {reachableCalculationResult}");

            foreach (var pair in reachables)
            {
                var reachableNodes = string.Join(", ", pair.Value.Select(node => $"#{node.ProjectInfo.Id}"));
                Console.WriteLine($"  #{pair.Key.ProjectInfo.Id}: {reachableNodes}");
            }

            Console.WriteLine();
        }

        private static void PrintBuild(
            Component.Build build,
            string header = "build:",
            bool printTargetGraph = false,
            bool printMessages = false,
            bool printWarnings = false,
            bool printErrors = false)
        {
            Console.WriteLine(header);

            foreach (var project in build.ProjectsById.Values)
            {
                Console.WriteLine($"  project #{project.Id}: {project.ProjectFile}");

                foreach (var target in project.OrderedTargets)
                {
                    Console.WriteLine($"    target #{target.Id} {target.Name}");

                    if (printTargetGraph)
                    {
                        Console.WriteLine($"      directly before this: {string.Join(";", target.Node_BeforeThis.AdjacentNodes.Select(beforeThis => beforeThis.TargetInfo.Name))}");
                        Console.WriteLine($"      directly after this: {string.Join(";", target.Node_AfterThis.AdjacentNodes.Select(afterThis => afterThis.TargetInfo.Name))}");
                    }

                    foreach (var task in target.OrderedTasks)
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

        private static void PrintAllLogs<T>(IReadOnlyList<T> logs, string header) where T : Log
        {
            Console.WriteLine(header);

            foreach (var log in logs)
            {
                Console.WriteLine($"  {log.Text}");
            }

            Console.WriteLine();
        }

        private static void PrintComponentLogs(
            Component.Component component,
            bool printMessages,
            bool printWarnings,
            bool printErrors,
            string indent)
        {
            PrintLogs(component.Messages, printMessages, indent);
            PrintLogs(component.Warnings, printWarnings, indent);
            PrintLogs(component.Errors, printErrors, indent);
        }

        private static void PrintLogs(IReadOnlyList<Log> logs, bool flag, string indent)
        {
            if (!flag)
            {
                return;
            }

            foreach (var log in logs.ToList())
            {
                Console.WriteLine($"{indent}{log.GetType().Name}: {log.Text}");
            }
        }

        private static void PrintTokens(string expression, string header = "tokens:")
        {
            Console.WriteLine(header);

            var scanner = new Scanner(expression);

            for (; !(scanner.Token is EofToken); scanner.ReadNextToken())
            {
                Console.WriteLine(scanner.Token + (scanner.Token is StringToken ? $" {(scanner.Token as StringToken).Value}" : ""));
            }

            Console.WriteLine();
        }

        private static void PrintAst(string expression, string header = "AST:")
        {
            Console.WriteLine(header);

            var ast = Parser.Parse(expression);

            for (; ast != null; ast = (ast as ComponentNode)?.Next)
            {
                Console.WriteLine(ast);
            }

            Console.WriteLine();
        }

        private static void PrintInterpResults(Component.Build build, string expression, string header = "interp result:")
        {
            Console.WriteLine(header);

            var ast = Parser.Parse(expression);
            var results = ast.Interpret(new[] { build });

            foreach (var result in results)
            {
                var text = "";
                if (result is Log log)
                {
                    text = log.Text;
                }
                else if (result is Project project)
                {
                    text = $"#{project.Id} {project.ProjectFile}";
                }
                else if (result is Target target)
                {
                    text = $"#{target.Id} {target.Name} {target.ParentProject.ProjectFile}";
                }
                else if (result is Task task)
                {
                    text = $"#{task.Id} {task.Name} {task.ParentTarget.Name} {task.ParentTarget.ParentProject.ProjectFile}";
                }

                Console.WriteLine($"  {result.ToString()}: {text}");
            }

            Console.WriteLine();
        }
    }
}