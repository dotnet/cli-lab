// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Reflection;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    internal static class BootstrapperCommandParser
    {
        public static Parser BootstrapParser;

        public static RootCommand BootstrapperRootCommand = new RootCommand("dotnet bootstrapper");

        public static readonly Command VersionCommand = new Command("--version");

        private static readonly Lazy<string> _assemblyVersion =
            new Lazy<string>(() =>
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                if (assemblyVersionAttribute == null)
                {
                    return assembly.GetName().Version.ToString();
                }
                else
                {
                    return assemblyVersionAttribute.InformationalVersion;
                }
            });

        static BootstrapperCommandParser()
        {
            BootstrapperRootCommand.AddCommand(VersionCommand);
            VersionCommand.Handler = CommandHandler.Create(() =>
            {
                Console.WriteLine(_assemblyVersion.Value);
            });

            BootstrapParser = new CommandLineBuilder(BootstrapperRootCommand)
                .UseDefaults()
                //  .UseHelpBuilder(context => new UninstallHelpBuilder(context.Console))
                .Build();
        }
    }
}
