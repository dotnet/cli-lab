using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Tests.Attributes;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Configs
{
    public class CommandLineConfigsTests
    {
        [Theory]
        [InlineData("list", new string[] { })]
        [InlineData("list --sdk", new string[] { "sdk" })]
        [InlineData("list --runtime", new string[] { "runtime" })]
        [InlineData("list --sdk --runtime", new string[] { "sdk", "runtime" })]
        [InlineData("list -v d", new string[] { "verbosity" })]
        [InlineData("list --verbosity diag", new string[] { "verbosity" })]
        [InlineData("list --sdk -v q", new string[] { "verbosity", "sdk" })]
        [InlineData("list --runtime --verbosity minimal", new string[] { "verbosity", "runtime" })]
        [InlineData("list --sdk --runtime -v normal", new string[] { "verbosity", "sdk", "runtime" })]
        internal void TestListCommandAccept(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.CommandResult.Name.Should().Be("list");
            parseResult.CommandResult.Tokens.Should().BeEmpty();

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.ListAuxOptions
                .Select(option => option.Name)
                .Where(option => parseResult.CommandResult.OptionResult(option) != null)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [WindowsOnlyTheory]
        [InlineData("list --aspnet-runtime", new string[] { "aspnet-runtime" })]
        [InlineData("list -v n --aspnet-runtime", new string[] { "verbosity", "aspnet-runtime" })]
        [InlineData("list --sdk --verbosity diag --aspnet-runtime", new string[] { "verbosity", "sdk", "aspnet-runtime" })]
        [InlineData("list --runtime --hosting-bundle", new string[] { "runtime", "hosting-bundle" })]
        internal void TestListCommandAcceptWindows(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.CommandResult.Name.Should().Be("list");
            parseResult.CommandResult.Tokens.Should().BeEmpty();

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.ListAuxOptions
                .Select(option => option.Name)
                .Where(option => parseResult.CommandResult.OptionResult(option) != null)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [Theory]
        [InlineData("list --all")]
        [InlineData("list --all-lower-patches")]
        [InlineData("list --all-but-latest")]
        [InlineData("list --all-but 2.2.300")]
        [InlineData("list --all-below 2.2.300")]
        [InlineData("list --all-previews")]
        [InlineData("list --all-previews-but-latest")]
        [InlineData("list --major-minor 2.2")]
        [InlineData("list 2.2")]
        [InlineData("list 2.2.300")]
        [InlineData("list --all --sdk")]
        [InlineData("list --all-but 2.2.5 --runtime")]
        [InlineData("list --major-minor 2.2 --sdk --runtime")]
        [InlineData("list -v")]
        [InlineData("list --verbosity")]
        [InlineData("list --all --sdk -v")]
        [InlineData("list --all-but 2.2.5 --verbosity --runtime")]
        [InlineData("list --major-minor 2.2 --sdk -v --runtime")]
        [InlineData("list --hosting-bundle --major-minor 1.1 --sdk")]
        [InlineData("list --version")]
        [InlineData("list -v q --version")]
        [InlineData("list --sdk --version")]
        [InlineData("list --sdk --runtime --version")]
        [InlineData("list --aspnet-runtime --version --hosting-bundle")]
        internal void TestListCommandReject(string command)
        {
            CommandLineConfigs.UninstallRootCommand.Parse(command).Errors
                .Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("--all")]
        [InlineData("--all-lower-patches")]
        [InlineData("--all-but-latest")]
        [InlineData("--all-but", "2.2.300", new[] { "2.2.300" })]
        [InlineData("--all-but", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [InlineData("--all-below", "2.2.300", "2.2.300")]
        [InlineData("--all-previews")]
        [InlineData("--all-previews-but-latest")]
        [InlineData("--major-minor", "2.2", "2.2")]
        [InlineData("", "2.2.300", new[] { "2.2.300" })] 
        [InlineData("", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [InlineData("", "--unknown-option", new[] { "--unknown-option" })]
        [InlineData("", "--unknown-option argument", new[] { "--unknown-option", "argument" })]
        internal void TestRemoveCommandAccept(string option, string argValue = "", object expected = null)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"remove {option} {argValue}");

            if (!option.Equals(string.Empty))
            {
                parseResult.CommandResult.OptionResult(option).Should().NotBeNull();
                parseResult.CommandResult.ValueForOption(option).Should().BeEquivalentTo(expected);
            }
            else
            {
                parseResult.CommandResult.Tokens.Select(t => t.Value).As<object>()
                    .Should().BeEquivalentTo(expected);
            }

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();
        }

        [Theory]
        [InlineData("--all")]
        [InlineData("--all-lower-patches")]
        [InlineData("--all-but-latest")]
        [InlineData("--all-but", "2.2.300", new[] { "2.2.300" })]
        [InlineData("--all-but", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [InlineData("--all-below", "2.2.300", "2.2.300")]
        [InlineData("--all-previews")]
        [InlineData("--all-previews-but-latest")]
        [InlineData("--major-minor", "2.2", "2.2")]
        [InlineData("", "2.2.300", new[] { "2.2.300" })]
        [InlineData("", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [InlineData("", "--unknown-option", new[] { "--unknown-option" })]
        [InlineData("", "--unknown-option argument", new[] { "--unknown-option", "argument" })]
        internal void TestDryRunCommandAccept(string option, string argValue = "", object expected = null)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"dry-run {option} {argValue}");

            if (!option.Equals(string.Empty))
            {
                parseResult.CommandResult.OptionResult(option).Should().NotBeNull();
                parseResult.CommandResult.ValueForOption(option).Should().BeEquivalentTo(expected);
            }
            else
            {
                parseResult.CommandResult.Tokens.Select(t => t.Value).As<object>()
                    .Should().BeEquivalentTo(expected);
            }

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();
        }

        [Theory]
        [InlineData("--all")]
        [InlineData("--all-lower-patches")]
        [InlineData("--all-but-latest")]
        [InlineData("--all-but", "2.2.300", new[] { "2.2.300" })]
        [InlineData("--all-but", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [InlineData("--all-below", "2.2.300", "2.2.300")]
        [InlineData("--all-previews")]
        [InlineData("--all-previews-but-latest")]
        [InlineData("--major-minor", "2.2", "2.2")]
        [InlineData("", "2.2.300", new[] { "2.2.300" })]
        [InlineData("", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [InlineData("", "--unknown-option", new[] { "--unknown-option" })]
        [InlineData("", "--unknown-option argument", new[] { "--unknown-option", "argument" })]
        internal void TestWhatIfCommandAccept(string option, string argValue = "", object expected = null)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"whatif {option} {argValue}");

            if (!option.Equals(string.Empty))
            {
                parseResult.CommandResult.OptionResult(option).Should().NotBeNull();
                parseResult.CommandResult.ValueForOption(option).Should().BeEquivalentTo(expected);
            }
            else
            {
                parseResult.CommandResult.Tokens.Select(t => t.Value).As<object>()
                    .Should().BeEquivalentTo(expected);
            }

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();
        }

        [Theory]
        [InlineData("--all --sdk", new string[] { "sdk" })]
        [InlineData("--all-below 2.2.300 --runtime", new string[] { "runtime" })]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk --runtime", new string[] { "sdk", "runtime" })]
        [InlineData("2.1.300 3.0.100-preview-276262-01 --verbosity diagnostic", new string[] { "verbosity" })]
        [InlineData("--all -v quiet --sdk", new string[] { "verbosity", "sdk" })]
        [InlineData("--major-minor 2.3 --verbosity m --runtime", new string[] { "verbosity", "runtime" })]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk -v n --runtime", new string[] { "verbosity", "sdk", "runtime" })]
        [InlineData("--all-below 2.2.300 --runtime --yes", new string[] { "runtime", "yes" })]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk -y --runtime", new string[] { "sdk", "runtime", "yes" })]
        internal void TestRemoveCommandAcceptAux(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("remove " + command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.RemoveAuxOptions
                .Select(option => option.Name)
                .Where(option => parseResult.CommandResult.OptionResult(option) != null)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [Theory]
        [InlineData("--all --sdk", new string[] { "sdk" })]
        [InlineData("--all-below 2.2.300 --runtime", new string[] { "runtime" })]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk --runtime", new string[] { "sdk", "runtime" })]
        [InlineData("2.1.300 3.0.100-preview-276262-01 --verbosity diagnostic", new string[] { "verbosity" })]
        [InlineData("--all -v quiet --sdk", new string[] { "verbosity", "sdk" })]
        [InlineData("--major-minor 2.3 --verbosity m --runtime", new string[] { "verbosity", "runtime" })]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk -v n --runtime", new string[] { "verbosity", "sdk", "runtime" })]
        internal void TestDryRunCommandAcceptAux(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("dry-run " + command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.RemoveAuxOptions
                .Select(option => option.Name)
                .Where(option => parseResult.CommandResult.OptionResult(option) != null)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [Theory]
        [InlineData("--all --sdk", new string[] { "sdk" })]
        [InlineData("--all-below 2.2.300 --runtime", new string[] { "runtime" })]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk --runtime", new string[] { "sdk", "runtime" })]
        [InlineData("2.1.300 3.0.100-preview-276262-01 --verbosity diagnostic", new string[] { "verbosity" })]
        [InlineData("--all -v quiet --sdk", new string[] { "verbosity", "sdk" })]
        [InlineData("--major-minor 2.3 --verbosity m --runtime", new string[] { "verbosity", "runtime" })]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk -v n --runtime", new string[] { "verbosity", "sdk", "runtime" })]
        internal void TestWhatIfCommandAcceptAux(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("whatif " + command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.RemoveAuxOptions
                .Select(option => option.Name)
                .Where(option => parseResult.CommandResult.OptionResult(option) != null)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [WindowsOnlyTheory]
        [InlineData("remove --all --sdk --aspnet-runtime", new string[] { "sdk", "aspnet-runtime" })]
        [InlineData("remove --major-minor 1.1 --hosting-bundle -v q", new string[] { "hosting-bundle", "verbosity" })]
        internal void TestOptionsAcceptAuxWindows(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.RemoveAuxOptions
                .Select(option => option.Name)
                .Where(option => parseResult.CommandResult.OptionResult(option) != null)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [Theory]
        [InlineData("")]
        internal void TestOptionsReject(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [Theory]
        [InlineData("--all-but")]
        [InlineData("--all-below")]
        [InlineData("--dry-run")]
        [InlineData("--major-minor")]
        [InlineData("--all-but --sdk")]
        [InlineData("--all-below --runtime")]
        [InlineData("--major-minor --verbosity q --sdk --runtime")]
        [InlineData("--verbosity")]
        [InlineData("-v")]
        [InlineData("--all-but 2.2.300 -v")]
        [InlineData("--all-below 1.23.456 -v")]
        [InlineData("--major-minor 3.0 --verbosity")]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0 --sdk --verbosity")]
        [InlineData("--dry-run --all-but")]
        [InlineData("--yes --verbosity")]
        [InlineData("--major-minor -y")]
        internal void TestRemoveCommandReject(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("remove " + command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [Theory]
        [InlineData("--all-but")]
        [InlineData("--yes")]
        [InlineData("--all-below")]
        [InlineData("--dry-run")]
        [InlineData("--major-minor")]
        [InlineData("--all-but --sdk")]
        [InlineData("--all-below --runtime")]
        [InlineData("--major-minor --verbosity q --sdk --runtime")]
        [InlineData("--verbosity")]
        [InlineData("-v")]
        [InlineData("--all-but 2.2.300 -v")]
        [InlineData("--all-below 1.23.456 -v")]
        [InlineData("--major-minor 3.0 --verbosity")]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0 --sdk --verbosity")]
        [InlineData("--dry-run --all-but")]
        [InlineData("--yes --verbosity")]
        internal void TestDryRunCommandReject(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("dry-run " + command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [Theory]
        [InlineData("--all-but")]
        [InlineData("--yes")]
        [InlineData("--all-below")]
        [InlineData("--dry-run")]
        [InlineData("--major-minor")]
        [InlineData("--all-but --sdk")]
        [InlineData("--all-below --runtime")]
        [InlineData("--major-minor --verbosity q --sdk --runtime")]
        [InlineData("--verbosity")]
        [InlineData("-v")]
        [InlineData("--all-but 2.2.300 -v")]
        [InlineData("--all-below 1.23.456 -v")]
        [InlineData("--major-minor 3.0 --verbosity")]
        [InlineData("--all-but 2.1.5 2.1.7 3.0.0 --sdk --verbosity")]
        [InlineData("--dry-run --all-but")]
        [InlineData("--yes --verbosity")]
        internal void TestWhatIfCommandReject(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("whatif " + command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [MacOsOnlyTheory]
        [InlineData("remove --all --aspnet-runtime")]
        [InlineData("remove --major-minor 1.1 --hosting-bundle")]
        [InlineData("remove --all-but 2.2.300 --sdk --aspnet-runtime -v q")]
        [InlineData("remove --all-below 2.2 --verbosity normal --sdk --hosting-bundle")]
        [InlineData("remove --major-minor 1.1 --hosting-bundle --dry-run")]
        [InlineData("remove --all-but 2.2.300 --sdk --aspnet-runtime --yes -v q")]
        [InlineData("remove --all-below 2.2 --verbosity normal -y --sdk --hosting-bundle")]
        internal void TestOptionsRejectMacOs(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [Theory]
        [InlineData("--all")]
        [InlineData("--all-lower-patches")]
        [InlineData("--all-but-latest")]
        [InlineData("--all-but", "2.2.300")]
        [InlineData("--all-but", "2.2.300 3.0.100")]
        [InlineData("--all-below", "2.2.300")]
        [InlineData("--all-previews")]
        [InlineData("--all-previews-but-latest")]
        [InlineData("--major-minor", "2.2")]
        [InlineData("", "2.2.300")] 
        [InlineData("", "2.2.300 3.0.100")]
        [InlineData("", "--unknown-option")]
        [InlineData("", "--unknown-option argument")]
        internal void TestGetUninstallRemoveOptionAccept(string option, string argValue = "")
        {
            var commandResult = CommandLineConfigs.UninstallRootCommand.Parse($"remove {option} {argValue}").CommandResult;

            if (option.Equals(string.Empty))
            {
                commandResult.GetUninstallMainOption().Should().BeNull();
            } 
            else
            {
                commandResult.GetUninstallMainOption().Name
                    .Should().Be(commandResult.OptionResult(option).Option.Name);
            }
        }

        [Theory]
        [InlineData("--all")]
        [InlineData("--all-lower-patches")]
        [InlineData("--all-but-latest")]
        [InlineData("--all-but", "2.2.300")]
        [InlineData("--all-but", "2.2.300 3.0.100")]
        [InlineData("--all-below", "2.2.300")]
        [InlineData("--all-previews")]
        [InlineData("--all-previews-but-latest")]
        [InlineData("--major-minor", "2.2")]
        [InlineData("", "2.2.300")]
        [InlineData("", "2.2.300 3.0.100")]
        [InlineData("", "--unknown-option")]
        [InlineData("", "--unknown-option argument")]
        internal void TestGetUninstallDryRunOptionAccept(string option, string argValue = "")
        {
            var commandResult = CommandLineConfigs.UninstallRootCommand.Parse($"dry-run {option} {argValue}").CommandResult;

            if (option.Equals(string.Empty))
            {
                commandResult.GetUninstallMainOption().Should().BeNull();
            }
            else
            {
                commandResult.GetUninstallMainOption().Name
                    .Should().Be(commandResult.OptionResult(option).Option.Name);
            }
        }

        [Theory]
        [InlineData("--all")]
        [InlineData("--all-lower-patches")]
        [InlineData("--all-but-latest")]
        [InlineData("--all-but", "2.2.300")]
        [InlineData("--all-but", "2.2.300 3.0.100")]
        [InlineData("--all-below", "2.2.300")]
        [InlineData("--all-previews")]
        [InlineData("--all-previews-but-latest")]
        [InlineData("--major-minor", "2.2")]
        [InlineData("", "2.2.300")]
        [InlineData("", "2.2.300 3.0.100")]
        [InlineData("", "--unknown-option")]
        [InlineData("", "--unknown-option argument")]
        internal void TestGetUninstallWhatIfOptionAccept(string option, string argValue = "")
        {
            var commandResult = CommandLineConfigs.UninstallRootCommand.Parse($"whatif {option} {argValue}").CommandResult;

            if (option.Equals(string.Empty))
            {
                commandResult.GetUninstallMainOption().Should().BeNull();
            }
            else
            {
                commandResult.GetUninstallMainOption().Name
                    .Should().Be(commandResult.OptionResult(option).Option.Name);
            }
        }

        public static IEnumerable<object[]> GetDataForTestGetUninstallMainOptionOptionsConflictException()
        {
            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-lower-patches", ArgValue: "")
            };

            yield return new object[]
            {
                (Command: "dry-run", Option: "--all", ArgValue: ""),
                (Command: "dry-run", Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Command: "whatif", Option: "--all", ArgValue: ""),
                (Command: "whatif", Option: "--all-but", ArgValue: "2.2.300 2.1.700")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--major-minor", ArgValue: "2.1")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100 --unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "--unknown-option"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.2.300"),
                (Command: "remove", Option: "--major-minor", ArgValue: "--unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-lower-patches", ArgValue: ""),
                (Command: "remove", Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.2.300"),
                (Command: "remove", Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 2.1.700"),
                (Command: "remove", Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--major-minor", ArgValue: "2.1"),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100"),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100 --unknown-option"),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100"),
                (Command: "remove", Option: "--all-below", ArgValue: "--unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--major-minor", ArgValue: "--unknown-option"),
                (Command: "remove", Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-lower-patches", ArgValue: ""),
                (Command: "remove", Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 --unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-but", ArgValue: "2.1.700 3.0.100"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 --unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 --unknown-option"),
                (Command: "remove", Option: "--all-lower-patches", ArgValue: ""),
                (Command: "remove", Option: "--major-minor", ArgValue: "2.2"),
                (Command: "remove", Option: "--all-previews-but-latest", ArgValue: "")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestGetUninstallMainOptionOptionsConflictException))]
        internal void TestGetUninstallMainOptionOptionsConflictException(params (string Command, string Option, string ArgValue)[] options)
        {
            var command = string.Join(" ", options.Select(option => $"{option.Command} {option.Option} {option.ArgValue}"));
            var optionNames = string.Join(", ", options.Select(option => $"--{option.Option}"));

            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse(command)
                .CommandResult.GetUninstallMainOption();

            action.Should().Throw<OptionsConflictException>(string.Format(LocalizableStrings.OptionsConflictExceptionMessageFormat, optionNames));
        }

        [Theory]
        [InlineData("remove", "--all", "2.2.300")]
        [InlineData("remove", "--all", "--unknown-option")]
        [InlineData("dry-run", "--all", "2.2.300")]
        [InlineData("whatif", "--all", "2.2.300")]
        internal void TestGetUninstallMainOptionMoreThanZeroVersionSpecifiedException(string command, string option, string commandArgValue)
        {
            Action action1 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {option} {commandArgValue}")
                .CommandResult.GetUninstallMainOption();

            Action action2 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {commandArgValue} {option}")
                .CommandResult.GetUninstallMainOption();

            action1.Should().Throw<MoreThanZeroVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanZeroVersionSpecifiedExceptionMessageFormat, option));
            action2.Should().Throw<MoreThanZeroVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanZeroVersionSpecifiedExceptionMessageFormat, option));
        }

        [Theory]
        [InlineData("remove", "--all-below", "2.1.700", "2.2.300")]
        [InlineData("whatif", "--all-below", "2.1.700", "2.2.300")]
        [InlineData("dry-run", "--all-below", "2.1.700", "2.2.300")]
        [InlineData("remove", "--all-below", "--unknown-option-1", "--unknown-option-2")]
        internal void TestGetUninstallMainOptionMoreThanOneVersionSpecifiedException(string command, string option, string commandArgValue, string optionArgValue)
        {
            Action action1 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {option} {optionArgValue} {commandArgValue}")
                .CommandResult.GetUninstallMainOption();

            Action action2 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {commandArgValue} {option} {optionArgValue}")
                .CommandResult.GetUninstallMainOption();

            action1.Should().Throw<MoreThanOneVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanOneVersionSpecifiedExceptionMessageFormat, option));
            action2.Should().Throw<MoreThanOneVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanOneVersionSpecifiedExceptionMessageFormat, option));
        }

        [Theory]
        [InlineData("remove", "--all-but", "2.2.300")]
        [InlineData("whatif", "--all-but", "2.2.300")]
        [InlineData("dry-run", "--all-but", "2.2.300")]
        [InlineData("remove", "--all-but", "2.2.300 2.1.700")]
        [InlineData("remove", "--all-but", "--unknown-option 2.1.700")]
        [InlineData("remove", "--all-but", "2.1.700 --unknown-option")]
        [InlineData("remove", "--all-but", "--unknown-option-1 --unknown-option-2")]
        [InlineData("remove", "--all-but", "2.2.300", "2.1.700")]
        [InlineData("remove", "--all-but", "2.2.300 2.1.700", "2.1.202 2.2.233")]
        [InlineData("remove", "--all-but", "--unknown-option 2.1.700", "--unknown-option-2")]
        [InlineData("remove", "--all-but", "2.1.700 --unknown-option", "--unknown-option-2 2.3.333")]
        [InlineData("remove", "--all-but", "--unknown-option-1 --unknown-option-2", "3.0.100")]
        internal void TestGetUninstallMainOptionVersionBeforeOptionException(string command, string option, string commandArgValue, string optionArgValue = "")
        {
            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {commandArgValue} {option} {optionArgValue}")
                .CommandResult.GetUninstallMainOption();

            action.Should().Throw<VersionBeforeOptionException>(string.Format(LocalizableStrings.VersionBeforeOptionExceptionMessageFormat, option));
        }

        [Theory]
        [InlineData("remove --sdk", BundleType.Sdk)]
        [InlineData("remove --runtime", BundleType.Runtime)]
        [InlineData("remove --sdk --runtime", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("remove --sdk --verbosity minimal", BundleType.Sdk)]
        [InlineData("remove -v normal --runtime", BundleType.Runtime)]
        [InlineData("remove --sdk --verbosity diag --runtime", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("remove --sdk --all-but 2.2.300 2.1.700", BundleType.Sdk)]
        [InlineData("remove --runtime --all-below 3.0.1-preview-10086", BundleType.Runtime)]
        [InlineData("remove --sdk --runtime --all-previews", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("remove --runtime --all", BundleType.Runtime)]
        [InlineData("remove --yes --sdk --all-below 2.0", BundleType.Sdk)]
        [InlineData("remove --sdk --runtime -y", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("dry-run --sdk --runtime", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("whatif --sdk --runtime", BundleType.Sdk | BundleType.Runtime)]
        internal void TestGetTypeSelectionRemoveCommand(string command, BundleType expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.CommandResult.GetTypeSelection()
                .Should().Be(expected);
        }

        [WindowsOnlyTheory]
        [InlineData("remove", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [InlineData("remove -v q", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [InlineData("remove --all", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [InlineData("remove --aspnet-runtime", BundleType.AspNetRuntime)]
        [InlineData("remove --sdk --aspnet-runtime --all-but 2.2.3", BundleType.Sdk | BundleType.AspNetRuntime)]
        [InlineData("remove --sdk --runtime --aspnet-runtime", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime)]
        [InlineData("remove --hosting-bundle --aspnet-runtime", BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [InlineData("remove --hosting-bundle --sdk --all", BundleType.Sdk | BundleType.HostingBundle)]
        [InlineData("remove --yes", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [InlineData("remove -y", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        internal void TestGetTypeSelectionRootCommandWindows(string command, BundleType expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.CommandResult.GetTypeSelection()
                .Should().Be(expected);
        }

        [MacOsOnlyTheory]
        [InlineData("remove", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("remove -v q", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("remove --all", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("remove --yes", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("remove -y", BundleType.Sdk | BundleType.Runtime)]
        internal void TestGetTypeSelectionRootCommandMacOs(string command, BundleType expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.CommandResult.GetTypeSelection()
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("remove 2.2.300 --sdk", VerbosityLevel.Normal)]
        [InlineData("remove --all -v q --sdk", VerbosityLevel.Quiet)]
        [InlineData("remove --all-below 2.2 --verbosity minimal --sdk", VerbosityLevel.Minimal)]
        [InlineData("remove --all-but 2.2.300 2.1.700 -v normal --runtime", VerbosityLevel.Normal)]
        [InlineData("remove --runtime --all-previews --verbosity d", VerbosityLevel.Detailed)]
        [InlineData("remove -v diag --runtime --major-minor 2.2", VerbosityLevel.Diagnostic)]
        [InlineData("remove -v diagnostic --major-minor 2.2 --runtime", VerbosityLevel.Diagnostic)]
        [InlineData("remove 1.1.11 -v normal --hosting-bundle", VerbosityLevel.Normal)]
        [InlineData("list", VerbosityLevel.Normal)]
        [InlineData("list -v q", VerbosityLevel.Quiet)]
        [InlineData("list --verbosity minimal", VerbosityLevel.Minimal)]
        [InlineData("list -v normal", VerbosityLevel.Normal)]
        [InlineData("list --verbosity d", VerbosityLevel.Detailed)]
        [InlineData("list -v diag", VerbosityLevel.Diagnostic)]
        [InlineData("list -v diagnostic", VerbosityLevel.Diagnostic)]
        [InlineData("list -v d", VerbosityLevel.Detailed)]
        [InlineData("remove --runtime --all-below 2.0 --yes -v q", VerbosityLevel.Quiet)]
        [InlineData("remove --runtime --all-previews --y -v diag", VerbosityLevel.Diagnostic)]
        internal void TestGetVerbosityLevel(string command, VerbosityLevel expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.CommandResult.GetVerbosityLevel()
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("remove 2.2.300 --sdk -v qu")]
        [InlineData("remove --all --sdk --verbosity mini")]
        [InlineData("remove --major-minor 2.1 -v unknown")]
        [InlineData("list -v qu")]
        [InlineData("list --verbosity mini")]
        [InlineData("list -v unknown")]
        internal void TestGetVerbosityLevelVerbosityLevelInvalidException(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);
            Action action = () => parseResult.CommandResult.GetVerbosityLevel();

            action.Should().Throw<VerbosityLevelInvalidException>(LocalizableStrings.VerbosityLevelInvalidExceptionMessage);
        }

        [Theory]
        [InlineData("remove --version")]
        [InlineData("remove --all --sdk --version")]
        [InlineData("remove 2.2.300 --runtime --version")]
        [InlineData("remove --version 2.2.300 --runtime")]
        [InlineData("remove --version --major-minor 2.1")]
        [InlineData("remove --version --all-but 2.2.300 2.1.700")]
        [InlineData("remove 2.2.5 --runtime -y --version")]
        [InlineData("remove --version --sdk 2.2.300 2.1.700 --yes")]
        internal void TestVersionOption(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.CommandResult.OptionResult(CommandLineConfigs.VersionOption.Name)
                .Should().NotBeNull();
        }
    }
}
