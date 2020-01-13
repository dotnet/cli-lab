using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Tests.Attributes;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.IntegrationTests
{
    public class IntegrationTests
    {
        private static readonly string ProtectedPattern = @"{0}\s+x\d*\s+\[";

        [WindowsOnlyTheory]
        [InlineData(new string[] { "--sdk" }, new string[] { "1.0.1", "3.1.0", "2.1.1" }, new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData(new string[] { "--runtime" }, new string[] { }, new string[] { "1.0.1", "3.1.0", "2.1.1", "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData(new string[] { "--sdk", "--x64" }, new string[] { "1.0.1" }, new string[] { "3.0.0", "3.0.0-preview", "1.0.0" })]
        internal void ListCommandExecutionIsCorrectOnWindows(string[] options, string[] expectedProtected, string[] expectedUninstallable)
        {
            ListCommandExecutionIsCorrect(options, expectedProtected, expectedUninstallable);
        }

        [MacOsOnlyTheory]
        [InlineData(new string[] { "--sdk" }, new string[] { "3.1.0" }, new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2", "2.1.1", "1.0.1" })]
        [InlineData(new string[] { "--runtime" }, new string[] { "3.1.0","3.0.2", "2.1.1", "1.0.1" }, new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2-preview1", "3.0.2-preview2" })]
        internal void ListCommandExecutionIsCorrectOnMac(string[] options, string[] expectedProtected, string[] expectedUninstallable)
        {
            ListCommandExecutionIsCorrect(options, expectedProtected, expectedUninstallable);
        }

        private void ListCommandExecutionIsCorrect(string[] options, string[] expectedProtected, string[] expectedUninstallable)
        {
            var bundleCollector = new MockBundleCollector();
            var terminal = new TestTerminal();
            CommandLineConfigs.UninstallRootCommand.Parse($"list");
            ListCommandExec.Execute(bundleCollector, terminal, new Region(0, 0, int.MaxValue, int.MaxValue), options);

            if (expectedProtected.Length != 0)
            {
                terminal.Out.ToString().Should().ContainAll(expectedProtected);
                // Check expected versions are marked as protected
                foreach (var version in expectedProtected)
                {
                    var patternWithVersion = string.Format(ProtectedPattern, version);
                    Regex.IsMatch(terminal.Out.ToString(), patternWithVersion).Should().BeTrue();
                }
            }
            if (expectedUninstallable.Length != 0)
            {
                terminal.Out.ToString().Should().ContainAll(expectedUninstallable);
                foreach (var version in expectedUninstallable)
                {
                    var patternWithVersion = string.Format(ProtectedPattern, version);
                    Regex.IsMatch(terminal.Out.ToString(), patternWithVersion).Should().BeFalse();
                }
            }
        }

        [WindowsOnlyTheory]
        [InlineData("--all --sdk", new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("--all --sdk --force", new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "1.0.1", "3.1.0", "2.1.1", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("--all --runtime", new string[] { "1.0.1", "3.1.0", "2.1.1", "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("--all --sdk --x64", new string[] { "3.0.0", "3.0.0-preview", "1.0.0" })]
        [InlineData("--all --sdk --x64 --force", new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "1.0.1" })]
        [InlineData("--all --sdk --x86", new string[] { "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("--all --sdk --x86 --force", new string[] { "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2", "2.1.1", "3.1.0" })]
        internal void DryRunCommandExecutionIsCorrectOnWindows(string options, string[] expectedBundles)
        {
            DryRunCommandExecutionIsCorrect(options, expectedBundles);
        }

        [MacOsOnlyTheory]
        [InlineData("--all --sdk", new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "2.1.1", "1.0.1", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("--all --runtime", new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("--all-previews --sdk", new string[] { "3.0.0-preview", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("--all-lower-patches --sdk", new string[] { "1.0.0", "3.0.1", "3.0.0", "3.0.0-preview", "3.0.2-preview1", "3.0.2-preview2" })]
        internal void DryRunCommandExecutionIsCorrectOnMac(string options, string[] expectedBundles)
        {
            DryRunCommandExecutionIsCorrect(options, expectedBundles);
        }

        private void DryRunCommandExecutionIsCorrect(string options, string[] expectedBundles)
        {
            var bundleCollector = new MockBundleCollector();
            var origConsole = Console.Out;
            var testConsole = new StringWriter();
            Console.SetOut(testConsole);
            try
            {
                CommandLineConfigs.CommandLineParseResult = CommandLineConfigs.UninstallRootCommand.Parse($"dry-run { options }");
                DryRunCommandExec.Execute(bundleCollector);
                var output = testConsole.GetStringBuilder().ToString();
                var remainingExpectedBundles = expectedBundles.Where(s => !output.Contains(s + "\r"));
                output.Should().ContainAll(remainingExpectedBundles.Select(s => s + "\n")); // Add whitespace to ensure x.x.x-preview and x.x.x can be differentiated

                // Check no extra bundles are in the output
                var unexpectedBundles = bundleCollector.GetAllInstalledBundles()
                    .Where(bundle => options.Contains(bundle.UninstallCommand)) // Bundles of the correct type
                    .Select(bundle => bundle.DisplayName)
                    .Where(bundle => !expectedBundles.Any(expected => expected.Contains(bundle)));
                if (unexpectedBundles.Count() != 0)
                {
                    output.Should().NotContainAny(unexpectedBundles); 
                }
            }
            finally
            {
                Console.SetOut(origConsole);
            }
        }

        [Fact]
        internal void VersionCommandOutputsVersionInfo()
        {
            var console = new TestConsole();
            var output = CommandLineConfigs.UninstallCommandParser.InvokeAsync(new string[1] { "--version" }, console);
            output.Result.Should().Be(0);
            console.Out.ToString().Should().Match("*.*.*");
        }
    }
}
