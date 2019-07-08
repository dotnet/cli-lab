using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Configs.Verbosity
{
    public class VerbosityHandlerTests
    {
        [Theory]
        [InlineData(VerbosityLevel.Quiet)]
        [InlineData(VerbosityLevel.Minimal)]
        [InlineData(VerbosityLevel.Normal)]
        [InlineData(VerbosityLevel.Detailed)]
        [InlineData(VerbosityLevel.Diagnostic)]
        internal void TestVerbosityHandlerNoRegistered(VerbosityLevel level)
        {
            var result = string.Empty;

            var verbosityHandler = new VerbosityHandler<object>();
            verbosityHandler.Execute(level, null);

            result.Should().Be(string.Empty);
        }

        [Theory]
        [InlineData(VerbosityLevel.Quiet)]
        [InlineData(VerbosityLevel.Minimal)]
        [InlineData(VerbosityLevel.Normal)]
        [InlineData(VerbosityLevel.Detailed)]
        [InlineData(VerbosityLevel.Diagnostic)]
        internal void TestVerbosityHandlerExactlyMatch(VerbosityLevel level)
        {
            var result = string.Empty;

            var verbosityHandler = new VerbosityHandler<object>();
            verbosityHandler.Register(level, obj => result = level.ToString());
            verbosityHandler.Execute(level, null);

            result.Should().Be(level.ToString());
        }

        [Theory]
        [InlineData(new VerbosityLevel[] { VerbosityLevel.Quiet }, VerbosityLevel.Normal, VerbosityLevel.Quiet)]
        [InlineData(new VerbosityLevel[] { VerbosityLevel.Quiet, VerbosityLevel.Normal }, VerbosityLevel.Minimal, VerbosityLevel.Quiet)]
        [InlineData(new VerbosityLevel[] { VerbosityLevel.Quiet, VerbosityLevel.Normal }, VerbosityLevel.Normal, VerbosityLevel.Normal)]
        [InlineData(new VerbosityLevel[] { VerbosityLevel.Quiet, VerbosityLevel.Normal }, VerbosityLevel.Detailed, VerbosityLevel.Normal)]
        [InlineData(new VerbosityLevel[] { VerbosityLevel.Quiet, VerbosityLevel.Normal, VerbosityLevel.Diagnostic }, VerbosityLevel.Quiet, VerbosityLevel.Quiet)]
        [InlineData(new VerbosityLevel[] { VerbosityLevel.Quiet, VerbosityLevel.Normal, VerbosityLevel.Diagnostic }, VerbosityLevel.Minimal, VerbosityLevel.Quiet)]
        [InlineData(new VerbosityLevel[] { VerbosityLevel.Quiet, VerbosityLevel.Normal, VerbosityLevel.Diagnostic }, VerbosityLevel.Detailed, VerbosityLevel.Normal)]
        [InlineData(new VerbosityLevel[] { VerbosityLevel.Quiet, VerbosityLevel.Normal, VerbosityLevel.Diagnostic }, VerbosityLevel.Diagnostic, VerbosityLevel.Diagnostic)]
        internal void TestVerbosityHandlerMatching(IEnumerable<VerbosityLevel> levels, VerbosityLevel executeLevel, VerbosityLevel expected)
        {
            var result = string.Empty;
            var verbosityHandler = new VerbosityHandler<object>();

            foreach (var level in levels)
            {
                verbosityHandler.Register(level, obj => result = level.ToString());
            }

            verbosityHandler.Execute(executeLevel, null);
            result.Should().Be(expected.ToString());
        }
    }
}
