// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Reflection;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    internal static class BootstrapperCommandParser
    {
        public static Parser BootstrapParser;

        public static RootCommand BootstrapperRootCommand = new RootCommand("dotnet bootstrapper");
        public static readonly Command HelpCommand = new("--help");

        static BootstrapperCommandParser()
        {
            BootstrapperRootCommand.AddCommand(CommandLineConfigs.VersionSubcommand);
            BootstrapperRootCommand.AddCommand(CommandLineConfigs.ListCommand);
            BootstrapperRootCommand.AddCommand(CommandLineConfigs.RemoveCommand);
            BootstrapperRootCommand.AddCommand(HelpCommand);
            HelpCommand.Handler = CommandHandler.Create(() =>
            {
                Console.WriteLine(LocalizableStrings.BootstrapperHelp);
            });

            BootstrapParser = new CommandLineBuilder(BootstrapperRootCommand)
                .UseDefaults()
                //  .UseHelpBuilder(context => new UninstallHelpBuilder(context.Console))
                .Build();
        }
    }
}
