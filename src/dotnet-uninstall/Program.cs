// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Microsoft.DotNet.Tools.Uninstall
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("dotnet-uninstall");

            var listCommand = new Command("list");
            var uninstallCommand = new Command("uninstall");
            rootCommand.Add(listCommand);
            rootCommand.Add(uninstallCommand);

            listCommand.Handler = CommandHandler.Create(() =>
            {
                Console.WriteLine("invoked list subcommand"); // TODO: implement list
            });

            uninstallCommand.Handler = CommandHandler.Create(() =>
            {
                Console.WriteLine("invoked uninstall subcommand"); // TODO: implement uninstall
            });

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
