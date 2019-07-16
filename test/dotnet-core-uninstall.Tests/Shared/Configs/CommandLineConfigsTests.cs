using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
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
        [InlineData("list --aspnet-runtime", new string[] { "aspnet-runtime" })]
        [InlineData("list -v n --aspnet-runtime", new string[] { "verbosity", "aspnet-runtime" })]
        [InlineData("list --sdk --verbosity diag --aspnet-runtime", new string[] { "verbosity", "sdk", "aspnet-runtime" })]
        internal void TestListCommandAccept(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.CommandResult.Name.Should().Be("list");
            parseResult.CommandResult.Tokens.Should().BeEmpty();

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.AuxOptions
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
        [InlineData("list --version")]
        [InlineData("list -v q --version")]
        [InlineData("list --sdk --version")]
        [InlineData("list --sdk --runtime --version")]
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
        internal void TestOptionsAccept(string option, string argValue = "", object expected = null)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"{option} {argValue}");

            if (!option.Equals(string.Empty))
            {
                parseResult.RootCommandResult.OptionResult(option).Should().NotBeNull();
                parseResult.RootCommandResult.ValueForOption(option).Should().BeEquivalentTo(expected);
            }
            else
            {
                parseResult.RootCommandResult.Tokens.Select(t => t.Value).As<object>()
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
        [InlineData("--all --sdk --aspnet-runtime", new string[] { "sdk", "aspnet-runtime" })]
        internal void TestOptionsAcceptAux(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.AuxOptions
                .Select(option => option.Name)
                .Where(option => parseResult.CommandResult.OptionResult(option) != null)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [Theory]
        [InlineData("--all-but")]
        [InlineData("--all-below")]
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
        internal void TestOptionsReject(string command)
        {
            CommandLineConfigs.UninstallRootCommand.Parse(command).Errors
                .Should().NotBeEmpty();
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
        internal void TestGetUninstallMainOptionAccept(string option, string argValue = "")
        {
            var rootCommandResult = CommandLineConfigs.UninstallRootCommand.Parse($"{option} {argValue}").RootCommandResult;

            rootCommandResult.GetUninstallMainOption()
                .Should().Be(option.Equals(string.Empty) ? null : rootCommandResult.OptionResult(option).Option);
        }

        public static IEnumerable<object[]> GetDataForTestGetUninstallMainOptionOptionsConflictException()
        {
            yield return new object[]
            {
                (Option: "--all", ArgValue: ""),
                (Option: "--all-lower-patches", ArgValue: "")
            };

            yield return new object[]
            {
                (Option: "--all", ArgValue: ""),
                (Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Option: "--all", ArgValue: ""),
                (Option: "--all-but", ArgValue: "2.2.300 2.1.700")
            };

            yield return new object[]
            {
                (Option: "--all-below", ArgValue: "2.1.700"),
                (Option: "--major-minor", ArgValue: "2.1")
            };

            yield return new object[]
            {
                (Option: "--all-below", ArgValue: "2.1.700"),
                (Option: "--all-but", ArgValue: "2.2.300 3.0.100")
            };

            yield return new object[]
            {
                (Option: "--all-below", ArgValue: "2.1.700"),
                (Option: "--all-but", ArgValue: "2.2.300 3.0.100 --unknown-option")
            };

            yield return new object[]
            {
                (Option: "--all-below", ArgValue: "--unknown-option"),
                (Option: "--all-but", ArgValue: "2.2.300 3.0.100")
            };

            yield return new object[]
            {
                (Option: "--all-below", ArgValue: "2.2.300"),
                (Option: "--major-minor", ArgValue: "--unknown-option")
            };

            yield return new object[]
            {
                (Option: "--all-lower-patches", ArgValue: ""),
                (Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Option: "--all-below", ArgValue: "2.2.300"),
                (Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Option: "--all-but", ArgValue: "2.2.300 2.1.700"),
                (Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Option: "--major-minor", ArgValue: "2.1"),
                (Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Option: "--all-but", ArgValue: "2.2.300 3.0.100"),
                (Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Option: "--all-but", ArgValue: "2.2.300 3.0.100 --unknown-option"),
                (Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Option: "--all-but", ArgValue: "2.2.300 3.0.100"),
                (Option: "--all-below", ArgValue: "--unknown-option")
            };

            yield return new object[]
            {
                (Option: "--major-minor", ArgValue: "--unknown-option"),
                (Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Option: "--all", ArgValue: ""),
                (Option: "--all-lower-patches", ArgValue: ""),
                (Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Option: "--all", ArgValue: ""),
                (Option: "--all-below", ArgValue: "2.1.700"),
                (Option: "--all-but", ArgValue: "2.2.300 --unknown-option")
            };

            yield return new object[]
            {
                (Option: "--all", ArgValue: ""),
                (Option: "--all-but", ArgValue: "2.1.700 3.0.100"),
                (Option: "--all-but", ArgValue: "2.2.300 --unknown-option")
            };

            yield return new object[]
            {
                (Option: "--all", ArgValue: ""),
                (Option: "--all-below", ArgValue: "2.1.700"),
                (Option: "--all-but", ArgValue: "2.2.300 --unknown-option"),
                (Option: "--all-lower-patches", ArgValue: ""),
                (Option: "--major-minor", ArgValue: "2.2"),
                (Option: "--all-previews-but-latest", ArgValue: "")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestGetUninstallMainOptionOptionsConflictException))]
        internal void TestGetUninstallMainOptionOptionsConflictException(params (string Option, string ArgValue)[] options)
        {
            var command = string.Join(" ", options.Select(option => $"{option.Option} {option.ArgValue}"));
            var optionNames = string.Join(", ", options.Select(option => $"--{option.Option}"));

            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse(command)
            .RootCommandResult.GetUninstallMainOption();

            action.Should().Throw<OptionsConflictException>(string.Format(LocalizableStrings.OptionsConflictExceptionMessageFormat, optionNames));
        }

        [Theory]
        [InlineData("--all", "2.2.300")]
        [InlineData("--all", "--unknown-option")]
        internal void TestGetUninstallMainOptionMoreThanZeroVersionSpecifiedException(string option, string commandArgValue)
        {
            Action action1 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{option} {commandArgValue}")
            .RootCommandResult.GetUninstallMainOption();

            Action action2 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{commandArgValue} {option}")
            .RootCommandResult.GetUninstallMainOption();

            action1.Should().Throw<MoreThanZeroVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanZeroVersionSpecifiedExceptionMessageFormat, option));
            action2.Should().Throw<MoreThanZeroVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanZeroVersionSpecifiedExceptionMessageFormat, option));
        }

        [Theory]
        [InlineData("--all-below", "2.1.700", "2.2.300")]
        [InlineData("--all-below", "--unknown-option-1", "--unknown-option-2")]
        internal void TestGetUninstallMainOptionMoreThanOneVersionSpecifiedException(string option, string commandArgValue, string optionArgValue)
        {
            Action action1 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{option} {optionArgValue} {commandArgValue}")
            .RootCommandResult.GetUninstallMainOption();

            Action action2 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{commandArgValue} {option} {optionArgValue}")
            .RootCommandResult.GetUninstallMainOption();

            action1.Should().Throw<MoreThanOneVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanOneVersionSpecifiedExceptionMessageFormat, option));
            action2.Should().Throw<MoreThanOneVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanOneVersionSpecifiedExceptionMessageFormat, option));
        }

        [Theory]
        [InlineData("--all-but", "2.2.300")]
        [InlineData("--all-but", "2.2.300 2.1.700")]
        [InlineData("--all-but", "--unknown-option 2.1.700")]
        [InlineData("--all-but", "2.1.700 --unknown-option")]
        [InlineData("--all-but", "--unknown-option-1 --unknown-option-2")]
        [InlineData("--all-but", "2.2.300", "2.1.700")]
        [InlineData("--all-but", "2.2.300 2.1.700", "2.1.202 2.2.233")]
        [InlineData("--all-but", "--unknown-option 2.1.700", "--unknown-option-2")]
        [InlineData("--all-but", "2.1.700 --unknown-option", "--unknown-option-2 2.3.333")]
        [InlineData("--all-but", "--unknown-option-1 --unknown-option-2", "3.0.100")]
        internal void TestGetUninstallMainOptionVersionBeforeOptionException(string option, string commandArgValue, string optionArgValue = "")
        {
            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse($"{commandArgValue} {option} {optionArgValue}")
            .RootCommandResult.GetUninstallMainOption();

            action.Should().Throw<VersionBeforeOptionException>(string.Format(LocalizableStrings.VersionBeforeOptionExceptionMessageFormat, option));
        }

        [Theory]
        [InlineData("", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime)]
        [InlineData("--sdk", BundleType.Sdk)]
        [InlineData("--runtime", BundleType.Runtime)]
        [InlineData("--sdk --runtime", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("-v q", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime)]
        [InlineData("--sdk --verbosity minimal", BundleType.Sdk)]
        [InlineData("-v normal --runtime", BundleType.Runtime)]
        [InlineData("--sdk --verbosity diag --runtime", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("--all", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime)]
        [InlineData("--sdk --all-but 2.2.300 2.1.700", BundleType.Sdk)]
        [InlineData("--runtime --all-below 3.0.1-preview-10086", BundleType.Runtime)]
        [InlineData("--sdk --runtime --all-previews", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("--aspnet-runtime", BundleType.AspNetRuntime)]
        [InlineData("--sdk --aspnet-runtime --all-but 2.2.3", BundleType.Sdk | BundleType.AspNetRuntime)]
        [InlineData("--sdk --runtime --aspnet-runtime", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime)]
        internal void TestGetTypeSelectionRootCommand(string command, BundleType expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.RootCommandResult.GetTypeSelection()
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("2.2.300 --sdk", VerbosityLevel.Normal)]
        [InlineData("--all -v q --sdk", VerbosityLevel.Quiet)]
        [InlineData("--all-below 2.2 --verbosity minimal --sdk", VerbosityLevel.Minimal)]
        [InlineData("--all-but 2.2.300 2.1.700 -v normal --runtime", VerbosityLevel.Normal)]
        [InlineData("--runtime --all-previews --verbosity d", VerbosityLevel.Detailed)]
        [InlineData("-v diag --runtime --major-minor 2.2", VerbosityLevel.Diagnostic)]
        [InlineData("-v diagnostic --major-minor 2.2 --runtime", VerbosityLevel.Diagnostic)]
        [InlineData("list", VerbosityLevel.Normal)]
        [InlineData("list -v q", VerbosityLevel.Quiet)]
        [InlineData("list --verbosity minimal", VerbosityLevel.Minimal)]
        [InlineData("list -v normal", VerbosityLevel.Normal)]
        [InlineData("list --verbosity d", VerbosityLevel.Detailed)]
        [InlineData("list -v diag", VerbosityLevel.Diagnostic)]
        [InlineData("list -v diagnostic", VerbosityLevel.Diagnostic)]
        [InlineData("-v q list", VerbosityLevel.Normal)]
        [InlineData("list -v d", VerbosityLevel.Detailed)]
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
        [InlineData("2.2.300 --sdk -v qu")]
        [InlineData("--all --sdk --verbosity mini")]
        [InlineData("--major-minor 2.1 -v unknown")]
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
        [InlineData("--version")]
        [InlineData("--all --sdk --version")]
        [InlineData("2.2.300 --runtime --version")]
        [InlineData("--version 2.2.300 --runtime")]
        [InlineData("--version --major-minor 2.1")]
        [InlineData("--version --all-but 2.2.300 2.1.700")]
        internal void TestVersionOption(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.RootCommandResult.OptionResult(CommandLineConfigs.VersionOption.Name)
                .Should().NotBeNull();
        }
    }
}
