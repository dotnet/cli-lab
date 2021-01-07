// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall
{
    internal class Program
    {
        internal static int Main(string[] args)
        {
            if (!(RuntimeInfo.RunningOnOSX || RuntimeInfo.RunningOnWindows))
            {
                throw new OperatingSystemNotSupportedException();
            }
            return CommandLineConfigs.UninstallCommandParser.InvokeAsync(args).Result;
        }
    }
}
