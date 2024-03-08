using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.IntegrationTests
{
    public class IntegrationTests
    {
        [Fact]
        internal async void VersionCommandOutputsVersionInfo()
        {
            var console = new TestConsole();
            var output = await CommandLineConfigs.UninstallCommandParser.InvokeAsync(new string[1] { "--version" }, console);
            output.Should().Be(0);
            console.Out.ToString().Should().Match("*.*.*");
        }
    }
}
