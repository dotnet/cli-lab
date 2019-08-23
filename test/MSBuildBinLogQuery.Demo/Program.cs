using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Logging.Query.Construction;
using Microsoft.Build.Logging.Query.Interpret;
using Microsoft.Build.Logging.Query.Result;

namespace MSBuildBinLogQuery.Demo
{
    class Program
    {
        internal static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintErrorMessage("Exactly two argument is required");
            }

            if (!File.Exists(args[0]))
            {
                PrintErrorMessage($"File not found: {args[0]}");
            }

            using var binaryLogReader = new BinaryLogReader(args[0]);
            var events = binaryLogReader.ReadEvents();

            var graphBuilder = new GraphBuilder();
            var build = graphBuilder.HandleEvents(events.ToArray());

            var interpreter = new Interpreter(args[1]);
            var results = interpreter.Filter(build);

            foreach (var result in results)
            {
                if (result is Project project)
                {
                    Console.WriteLine(project.Path);
                }
                else if (result is Target target)
                {
                    Console.WriteLine(target.Name);
                }
                else if (result is Task task)
                {
                    Console.WriteLine(task.Name);
                }
                else if (result is Log log)
                {
                    Console.WriteLine($"{log.Text} Task:{(log.Parent as Task).Name}");
                }
                else
                {
                    throw new NotImplementedException();
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
