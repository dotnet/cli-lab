using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace Microsoft.DotNet.Tools.Bootstrapper.Commands.List
{
    internal class ListCommandParser
    {
        internal static Argument<string> ChannelArgument = new Argument<string>(
            name: "channel",
            description: "The channel to list sdks for. If not specified, all sdks will be listed.")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        internal static Option<bool> AllowPreviews = new Option<bool>(
            "--allow-previews",
            description: "Allow preview releases to be listed.");

        private static readonly Command Command = ConstructCommand();
        public static Command GetCommand() => Command;
        private static Command ConstructCommand()
        {
            Command command = new("list", "List all sdks available for installation");
            command.AddArgument(ChannelArgument);
            command.AddOption(AllowPreviews);

            command.Handler = CommandHandler.Create((ParseResult parseResult) =>
            {
                return new ListCommand(parseResult).Execute();
            });

            return command;
        }
    }
}
