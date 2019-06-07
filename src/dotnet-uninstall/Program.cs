// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.Windows;

namespace Microsoft.DotNet.Tools.Uninstall
{
    internal class Program
    {
        internal static readonly bool RunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        internal static readonly bool RunningOnOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        internal static readonly bool RunningOnLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("dotnet-uninstall");

            var listCommand = new Command("list");

            rootCommand.Add(listCommand);

            var uninstallAll = new Option(
                "--all",
                Messages.UninstallAllOptionDescription,
                new Argument<bool>());

            rootCommand.AddOption(uninstallAll);

            listCommand.Handler = CommandHandler.Create(() =>
            {
                ExecuteListCommand();
            });

            rootCommand.Handler = CommandHandler.Create<bool>((all) =>
            {
                var options = new Dictionary<string, object>();

                options.Add("all", all);

                ExecuteUninstallCommand(rootCommand.Parse(args));
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        private static void ExecuteListCommand()
        {
            if (RunningOnWindows)
            {
                ListCommand.Execute();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void ExecuteUninstallCommand(ParseResult parseResult)
        {
            if (RunningOnWindows)
            {
                UninstallCommand.Execute(parseResult);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
