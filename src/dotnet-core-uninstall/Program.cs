// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall
{
    internal class Program
    {
        internal static async Task<int> Main(string[] args)
        {
            if (!(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)))
            {
                throw new OperatingSystemNotSupportedException();
            }
            return await CommandLineConfigs.UninstallCommandParser.InvokeAsync(args);
        }
    }
}
