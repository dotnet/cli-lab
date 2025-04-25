using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace Microsoft.DotNet.Tools.Bootstrapper.Commands.Search;

internal class SearchCommandParser
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
        Command command = new("search", "Search for SDKs available for installation.");
        command.AddArgument(ChannelArgument);
        command.AddOption(AllowPreviews);

        command.Handler = CommandHandler.Create((ParseResult parseResult) =>
        {
            return new SearchCommand(parseResult).Execute();
        });

        return command;
    }
}
