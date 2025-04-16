// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Microsoft.DotNet.Tools.Bootstrapper.Tests;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    public class EndToEndBootstrapperTest
    {
        private readonly static string artifactsDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "artifacts"));
        private readonly static string executablePath = Path.Combine(artifactsDirectory, "bin", "dotnet-bootstrapper", TestUtilities.GetConfiguration(), TestUtilities.GetTargetFramework(), "dotnet-bootstrapper.exe");

        [Fact]
        internal void ItReturnsZeroOnExit()
        {
            File.Exists(executablePath).Should().BeTrue($"Expected the executable to exist at {executablePath}" +
                $"Files in directory: ${String.Join(", ", Directory.GetFileSystemEntries(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(executablePath).ToString()).ToString()).ToString()).ToString()))}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
            process.ExitCode.Should().Be(0, "The bootstrapper should exit with a code of 0.");
        }

    }
}
