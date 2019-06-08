using System.CommandLine;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared
{
    public class FiltererTests
    {

        [Fact]
        public void TestIntExcluder()
        {
            var excluder = new Filterer<int, int>((value, list) => list.Where(element => element != value));
            excluder.InternalFilterer(1, new[] { 1, 3, 2, 2, 3, 1, 3 })
                .Should().BeEquivalentTo(new[] { 3, 2, 2, 3, 3 });
        }

        [Fact]
        public void TestStringLengthFilterer()
        {
            var filterer = new Filterer<int, string>((value, list) => list.Where(element => element.Length < value));
            filterer.InternalFilterer(8, new[] { "dotnet", "Microsoft", "hello", ".NET", "C#", "TypeScript" })
                .Should().BeEquivalentTo(new[] { "dotnet", "hello", ".NET", "C#" });
        }

        [Fact]
        public void TestMixedTypeFilterers()
        {
            var intFilterer = new Filterer<int, int>((value, list) => list.Where(element => element != value));
            var stringFilterer = new Filterer<string, int>((value, list) => list.Where(element => element > value.Length));

            var filterers = new Filterer<int>[] { intFilterer, stringFilterer };

            var intOption = new Option("--an-int", argument: new Argument<int>());
            var stringOption = new Option("--a-string", argument: new Argument<string>());
            var command = new Command("command");
            command.AddOption(intOption);
            command.AddOption(stringOption);
            var parseResult = command.Parse("command --an-int 2 --a-string HelloWorld");

            filterers[0].Filter(parseResult, "--an-int", new[] { 1, 3, 2, 2, 3, 1, 3 })
                .Should().BeEquivalentTo(new[] { 1, 3, 3, 1, 3 });

            filterers[1].Filter(parseResult, "--a-string", new[] { 1, 1, 2, 3, 5, 8, 13, 21 })
                .Should().BeEquivalentTo(new[] { 13, 21 });
        }

        [Fact]
        public void TestEmptyFilteredResult()
        {
            var filterer = new Filterer<bool, int>((value, list) => list.Where(element => false));

            filterer.InternalFilterer(true, new[] { 1, 3, 2, 2, 3, 1, 3 })
                .Should().BeEmpty();
        }

        [Fact]
        public void TestFullFilteredResult()
        {
            var filterer = new Filterer<bool, int>((value, list) => list.Where(element => true));

            filterer.InternalFilterer(true, new[] { 1, 3, 2, 2, 3, 1, 3 })
                .Should().BeEquivalentTo(new[] { 1, 3, 2, 2, 3, 1, 3 });
        }
    }
}
