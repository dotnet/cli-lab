using System;
using System.Collections.Generic;
using System.CommandLine;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared
{
    public class UninstallCommandTests
    {
        private readonly Command _command;

        public UninstallCommandTests()
        {
            var allOption = new Option("--all", argument: new Argument<bool>());
            var allLowerPatchesOption = new Option("--all-lower-patches", argument: new Argument<bool>());
            var allBelow = new Option("--all-below", argument: new Argument<string>());
            var allBut = new Option("--all-but", argument: new Argument<IEnumerable<string>>());
            _command = new Command("command", argument: new Argument<IEnumerable<string>>());
            _command.AddOption(allOption);
            _command.AddOption(allLowerPatchesOption);
            _command.AddOption(allBelow);
            _command.AddOption(allBut);
        }

        [Theory]
        [InlineData("command --all --all-lower-patches")]
        [InlineData("command --all-below 2.2.300 --all")]
        [InlineData("command --all-but 2.2.300 2.1.700 --all-below 1.1")]
        [InlineData("command --all-below 2.2 2.3")]
        [InlineData("command --all 2.2 2.3")]
        [InlineData("command 2.2.300 2.1.700 --all")]
        [InlineData("command 2.2.300 2.1.700 --all-below 2.2")]
        public void TestAssertCorrectOptionsConflict(string commandString)
        {
            var parseResult = _command.Parse(commandString);

            var action = (Action)(() => UninstallCommand<int>.AssertCorrectOptions(parseResult));
            action.Should().Throw<UninstallCommand<int>.OptionsConflictException>();
        }

        [Theory]
        [InlineData("command")]
        [InlineData("command 2.2.300")]
        [InlineData("command 2.2.300 2.1.700")]
        [InlineData("command --all")]
        [InlineData("command --all-but 2.2.300 2.1.700")]
        [InlineData("command --all-below 2.2")]
        [InlineData("command --all-but")]
        [InlineData("command --all-below")]
        public void TestAssertCorrectOptionsNoConflict(string commandString)
        {
            var parseResult = _command.Parse(commandString);

            var action = (Action)(() => UninstallCommand<int>.AssertCorrectOptions(parseResult));
            action.Should().NotThrow<UninstallCommand<int>.OptionsConflictException>();
        }
    }
}
