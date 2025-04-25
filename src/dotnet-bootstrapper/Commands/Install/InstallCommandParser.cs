using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace Microsoft.DotNet.Tools.Bootstrapper.Commands.Install;

internal class InstallCommandParser
{
    internal static Argument<string> ChannelArgument = new Argument<string>(
        name: "channel",
        description: "The channel to install sdks for. If not specified, It will take the latest.")
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    private static readonly Command Command = ConstructCommand();

    public static Command GetCommand() => Command;

    private static Command ConstructCommand()
    {
        Command command = new("install", "Install SDKs available for installation.");
        command.AddArgument(ChannelArgument);
        command.Handler = CommandHandler.Create((ParseResult parseResult) =>
        {
            return new InstallCommand(parseResult).Execute();
        });
        return command;
    }
}
