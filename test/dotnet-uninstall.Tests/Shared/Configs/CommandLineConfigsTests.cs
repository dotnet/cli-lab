using System;
using System.CommandLine;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Configs
{
    public class CommandLineConfigsTests
    {
        [Fact]
        public void TestListCommandAccept()
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("list");

            parseResult.CommandResult.Name.Should().Be("list");
            parseResult.CommandResult.Arguments.Should().BeEmpty();

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();
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
        public void TestListCommandReject(string command)
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
        public void TestOptionsAccept(string option, string argValue = "", object expected = null)
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
        [InlineData("--all-but")]
        [InlineData("--all-below")]
        [InlineData("--major-minor")]
        public void TestOptionsReject(string command)
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
        public void TestGetUniqueOptionAccept(string option, string argValue = "")
        {
            var rootCommandResult = CommandLineConfigs.UninstallRootCommand.Parse($"{option} {argValue}").RootCommandResult;

            rootCommandResult.GetUniqueOption()
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
        public void TestGetUniqueOptionRejectOptionsConflictException(string option1, string option2, string argValue1 = "", string argValue2 = "")
        {
            Action action1 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{option1} {argValue1} {option2} {argValue2}")
            .RootCommandResult.GetUniqueOption();

            Action action2 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{option2} {argValue2} {option1} {argValue1}")
            .RootCommandResult.GetUniqueOption();

            action1.Should().Throw<OptionsConflictException>(string.Format(Messages.OptionsConflictExceptionMessageFormat, option1, option2));
            action2.Should().Throw<OptionsConflictException>(string.Format(Messages.OptionsConflictExceptionMessageFormat, option1, option2));
        }

        [Theory]
        [InlineData("--all", "2.2.300")]
        [InlineData("--all-below", "2.1.700", "2.2.300")]
        [InlineData("--all", "--unknown-option")]
        [InlineData("--all-below", "--unknown-option-1", "--unknown-option-2")]
        public void TestGetUniqueOptionRejectCommandArgOptionConflictExceptionCommandArgAfter(string option, string commandArgValue, string optionArgValue = "")
        {
            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse($"{option} {optionArgValue} {commandArgValue}")
            .RootCommandResult.GetUniqueOption();

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
        public void TestGetUniqueOptionRejectCommandArgOptionConflictExceptionCommandArgBefore(string option, string commandArgValue, string optionArgValue = "")
        {
            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse($"{commandArgValue} {option} {optionArgValue}")
            .RootCommandResult.GetUniqueOption();

            action.Should().Throw<CommandArgOptionConflictException>(string.Format(Messages.CommandArgOptionConflictExceptionMessageFormat, option));
        }
    }
}
