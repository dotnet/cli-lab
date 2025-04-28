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
    internal static Argument<string> VersionArgument = new Argument<string>(
        name: "version",
        description: "SDK version to install. If not specified, It will take the latest.")
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    internal static Option<bool> AllowPreviews = new Option<bool>(
        "--allow-previews",
        description: "Allow preview releases to be installed.");

    private static readonly Command Command = ConstructCommand();

    public static Command GetCommand() => Command;

    private static Command ConstructCommand()
    {
        Command command = new("install", "Install SDKs available for installation.");

        command.AddArgument(VersionArgument);

        command.AddOption(AllowPreviews);

        command.Handler = CommandHandler.Create((ParseResult parseResult) =>
        {
            return new InstallCommand(parseResult).Execute();
        });
        return command;
    }
}
