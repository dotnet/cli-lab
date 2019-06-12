// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Runtime.InteropServices;

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
            var uninstallCommand = new Command("uninstall");
            rootCommand.Add(listCommand);
            rootCommand.Add(uninstallCommand);

            listCommand.Handler = CommandHandler.Create(() =>
            {
                ExecuteListCommand();
            });

            uninstallCommand.Handler = CommandHandler.Create((Func<int>)(() =>
            {
                throw new NotImplementedException();
            }));

            return rootCommand.InvokeAsync(args).Result;
        }

        private static void ExecuteListCommand()
        {
            if (RunningOnWindows)
            {
                Windows.ListCommand.Execute();
            }
            else
            {
                Console.WriteLine(LocalizableStrings.DoesNotSupportLinux);
                throw new NotImplementedException();
            }
        }
    }
}
