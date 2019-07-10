using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Configs.Verbosity
{
    public class VerbosityLoggerTests
    {
        private static readonly VerbosityLevel DefaultVerbosityLevel = VerbosityLevel.Normal;
        private static readonly string DefaultMessage = string.Empty;

        [Theory]
        [InlineData(VerbosityLevel.Quiet)]
        [InlineData(VerbosityLevel.Minimal)]
        [InlineData(VerbosityLevel.Normal)]
        [InlineData(VerbosityLevel.Detailed)]
        [InlineData(VerbosityLevel.Diagnostic)]
        internal void TestConstructor(VerbosityLevel level)
        {
            var logger = new VerbosityLogger(level);
            logger.Level.Should().Be(level);
        }

        [Theory]
        [InlineData(VerbosityLevel.Diagnostic + 1)]
        [InlineData(VerbosityLevel.Diagnostic + 2)]
        [InlineData(VerbosityLevel.Diagnostic + 10)]
        internal void TestConstructorArgumentOutOfRangeException(VerbosityLevel level)
        {
            Action action = () => new VerbosityLogger(level);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(VerbosityLevel.Minimal)]
        [InlineData(VerbosityLevel.Normal)]
        [InlineData(VerbosityLevel.Detailed)]
        [InlineData(VerbosityLevel.Diagnostic)]
        internal void TestLog(VerbosityLevel level)
        {
            Action action = () => new VerbosityLogger(DefaultVerbosityLevel).Log(level, DefaultMessage);
            action.Should().NotThrow<Exception>();
        }

        [Theory]
        [InlineData(VerbosityLevel.Quiet)]
        [InlineData(VerbosityLevel.Diagnostic + 1)]
        [InlineData(VerbosityLevel.Diagnostic + 2)]
        [InlineData(VerbosityLevel.Diagnostic + 10)]
        internal void TestLogArgumentOutOfRangeException(VerbosityLevel level)
        {
            Action action = () => new VerbosityLogger(DefaultVerbosityLevel).Log(level, DefaultMessage);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
