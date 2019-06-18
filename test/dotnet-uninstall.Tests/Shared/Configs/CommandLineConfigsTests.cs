using System;
using System.CommandLine;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
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
        internal void TestListCommandAccept(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.CommandResult.Name.Should().Be("list");
            parseResult.CommandResult.Arguments.Should().BeEmpty();

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
                parseResult.RootCommandResult.Arguments.As<object>().Should().BeEquivalentTo(expected);
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
        internal void TestOptionsAcceptAux(string options, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(options);

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
        internal void TestOptionsReject(string options)
        {
            CommandLineConfigs.UninstallRootCommand.Parse(options).Errors
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

        [Theory]
        [InlineData("--all", "--all-lower-patches")]
        [InlineData("--all", "--all-below", "", "2.2.300")]
        [InlineData("--all", "--all-but", "", "2.2.300 2.1.700")]
        [InlineData("--all-below", "--major-minor", "2.2.300", "2.1")]
        [InlineData("--all-below", "--all-but", "2.2.300", "2.1.700 3.0.100")]
        [InlineData("--all-below", "--all-but", "2.2.300", "2.1.700 3.0.100 --unknown-option")]
        [InlineData("--all-below", "--all-but", "--unknown-option", "2.1.700 3.0.100")]
        [InlineData("--all-below", "--major-minor", "2.2.300", "--unknown-option")]
        internal void TestGetUninstallMainOptionOptionsConflictException(string option1, string option2, string argValue1 = "", string argValue2 = "")
        {
            Action action1 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{option1} {argValue1} {option2} {argValue2}")
            .RootCommandResult.GetUninstallMainOption();

            Action action2 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{option2} {argValue2} {option1} {argValue1}")
            .RootCommandResult.GetUninstallMainOption();

            action1.Should().Throw<OptionsConflictException>(string.Format(Messages.OptionsConflictExceptionMessageFormat, option1, option2));
            action2.Should().Throw<OptionsConflictException>(string.Format(Messages.OptionsConflictExceptionMessageFormat, option1, option2));
        }

        [Theory]
        [InlineData("--all", "2.2.300")]
        [InlineData("--all-below", "2.1.700", "2.2.300")]
        [InlineData("--all", "--unknown-option")]
        [InlineData("--all-below", "--unknown-option-1", "--unknown-option-2")]
        internal void TestGetUninstallMainOptionCommandArgOptionConflictExceptionCommandArgAfter(string option, string commandArgValue, string optionArgValue = "")
        {
            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse($"{option} {optionArgValue} {commandArgValue}")
            .RootCommandResult.GetUninstallMainOption();

            action.Should().Throw<CommandArgOptionConflictException>(string.Format(Messages.CommandArgOptionConflictExceptionMessageFormat, option));
        }

        [Theory]
        [InlineData("--all", "2.2.300")]
        [InlineData("--all-below", "2.1.700", "2.2.300")]
        [InlineData("--all-but", "2.2.300 2.1.700")]
        [InlineData("--all", "--unknown-option")]
        [InlineData("--all-below", "--unknown-option-1", "--unknown-option-2")]
        [InlineData("--all-but", "--unknown-option 2.1.700")]
        [InlineData("--all-but", "2.1.700 --unknown-option")]
        [InlineData("--all-but", "--unknown-option-1 --unknown-option-2")]
        internal void TestGetUninstallMainOptionCommandArgOptionConflictExceptionCommandArgBefore(string option, string commandArgValue, string optionArgValue = "")
        {
            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse($"{commandArgValue} {option} {optionArgValue}")
            .RootCommandResult.GetUninstallMainOption();

            action.Should().Throw<CommandArgOptionConflictException>(string.Format(Messages.CommandArgOptionConflictExceptionMessageFormat, option));
        }

        [Theory]
        [InlineData("", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("--sdk", BundleType.Sdk)]
        [InlineData("--runtime", BundleType.Runtime)]
        [InlineData("--sdk --runtime", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("-v q", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("--sdk --verbosity minimal", BundleType.Sdk)]
        [InlineData("-v normal --runtime", BundleType.Runtime)]
        [InlineData("--sdk --verbosity diag --runtime", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("--all", BundleType.Sdk | BundleType.Runtime)]
        [InlineData("--sdk --all-but 2.2.300 2.1.700", BundleType.Sdk)]
        [InlineData("--runtime --all-below 3.0.1-preview-10086", BundleType.Runtime)]
        [InlineData("--sdk --runtime --all-previews", BundleType.Sdk | BundleType.Runtime)]
        internal void TestGetTypeSelectionRootCommand(string options, BundleType expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(options);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.RootCommandResult.GetTypeSelection()
                .Should().Be(expected);
        }
    }
}
