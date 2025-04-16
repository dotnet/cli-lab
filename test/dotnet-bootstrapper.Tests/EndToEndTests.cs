// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Microsoft.DotNet.Tools.Bootstrapper.Tests;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    public class EndToEndBootstrapperTest
    {
        private readonly static string TargetFramework = TestUtilities.GetTargetFramework();
        private readonly static string RuntimeIdentifier = TestUtilities.GetRuntimeIdentifier();
        private readonly static string artifactsDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "artifacts"));
        private readonly static string executablePath = Path.Combine(artifactsDirectory, "bin", "dotnet-bootstrapper", "Debug", TargetFramework, RuntimeIdentifier, "dotnet-bootstrapper.exe");

        [Fact]
        internal void ItReturnsZeroOnExit()
        {
            File.Exists(executablePath).Should().BeTrue($"Expected the executable to exist at {executablePath}");

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
            process.ExitCode.Should().Be(0, "The process should exit with a code of 0.");
        }

    }
}
