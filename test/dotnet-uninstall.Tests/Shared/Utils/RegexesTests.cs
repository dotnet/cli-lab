using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Utils
{
    public class RegexesTests
    {
        [Fact]
        public void TestDotNetCorePublisherRegexAccept()
        {
            Regexes.DotNetCorePublisherRegex.IsMatch("Microsoft Corporation")
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("microsoft corporation")]
        [InlineData("MICROSOFT CORPORATION")]
        [InlineData("Microsoft  Corporation")]
        [InlineData(" Microsoft Corporation")]
        [InlineData("Microsoft Corporation ")]
        [InlineData("Microsoft")]
        [InlineData("HelloMicrosoft CorporationWorld")]
        [InlineData("Hello Microsoft Corporation World")]
        public void TestDotNetCorePublisherRegexReject(string s)
        {
            Regexes.DotNetCorePublisherRegex.IsMatch(s)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("2.2.5", "2", "2", "5", "")]
        [InlineData("3.0.0 Preview 5", "3", "0", "0", "5")]
        [InlineData("2.2.300", "2", "2", "300", "")]
        [InlineData("3.0.100 - preview5", "3", "0", "100", "5")]
        [InlineData("3.0.100-preview5", "3", "0", "100", "5")]
        [InlineData("03.0.100 - preview5", "03", "0", "100", "5")]
        [InlineData("3.00.100 - preview5", "3", "00", "100", "5")]
        [InlineData("3.0.0100 - preview5", "3", "0", "0100", "5")]
        [InlineData("3.0.100 - preview05", "3", "0", "100", "05")]
        [InlineData("03.0.0 Preview 5", "03", "0", "0", "5")]
        [InlineData("3.00.0 Preview 5", "3", "00", "0", "5")]
        [InlineData("3.0.00 Preview 5", "3", "0", "00", "5")]
        [InlineData("3.0.0 Preview 05", "3", "0", "0", "05")]
        [InlineData("3.0.100 Preview 5", "3", "0", "100", "5")]
        public void TestDotNetCoreVersionExtractionRegexAccept(string s, string major, string minor, string patch, string preview)
        {
            var match = Regexes.DotNetCoreVersionExtractionRegex.Match(s);

            match.Groups[Regexes.DotNetCoreExtractionRegexMajorGroupName].Value
                .Should().Be(major);

            match.Groups[Regexes.DotNetCoreExtractionRegexMinorGroupName].Value
                .Should().Be(minor);

            match.Groups[Regexes.DotNetCoreExtractionRegexPatchGroupName].Value
                .Should().Be(patch);

            match.Groups[Regexes.DotNetCoreExtractionRegexPreviewGroupName].Value
                .Should().Be(preview);
        }

        [Theory]
        [InlineData("3.0.0 - preview5")]
        [InlineData("3.0.10 - preview5")]
        [InlineData("3.0.0 - preview 5")]
        [InlineData("a.0.0")]
        [InlineData("0.a.0")]
        [InlineData("0.0.a")]
        [InlineData("2.2.300 - preview")]
        [InlineData("3.0.0 Preview")]
        [InlineData("2.2.300 - previewA")]
        [InlineData("3.0.0 Preview A")]
        [InlineData("2.2.300 - preview 5")]
        [InlineData("2.2.300 - Preview5")]
        [InlineData("3.0.0 Preview5")]
        [InlineData("3.0.0 preview 5")]
        public void TestDotNetCoreVersionExtractionRegexReject(string s)
        {
            Regexes.DotNetCoreVersionExtractionRegex.IsMatch(s)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("2.2", "2", "2")]
        [InlineData("3.0", "3", "0")]
        [InlineData("03.0", "03", "0")]
        [InlineData("3.00", "3", "00")]
        public void TestDotNetCoreMajorMinorExtractionRegexAccept(string s, string major, string minor)
        {
            var match = Regexes.DotNetCoreMajorMinorExtractionRegex.Match(s);

            match.Groups[Regexes.DotNetCoreExtractionRegexMajorGroupName].Value
                .Should().Be(major);

            match.Groups[Regexes.DotNetCoreExtractionRegexMinorGroupName].Value
                .Should().Be(minor);
        }

        [Theory]
        [InlineData("a.0")]
        [InlineData("0.a")]
        [InlineData("2,2")]
        public void TestDotNetCoreMajorMinorExtractionRegexReject(string s)
        {
            Regexes.DotNetCoreMajorMinorExtractionRegex.IsMatch(s)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x64)", "2.2.300")]
        [InlineData("Microsoft .NET Core SDK 3.0.100-preview5 (x64)", "3.0.100-preview5")]
        [InlineData("Microsoft .NET Core SDK 3.0.100 - preview5 (x64)", "3.0.100 - preview5")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5 (x86)", "2.2.5")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.0 Preview 5 (x86)", "3.0.0 Preview 5")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.100 Preview 5 (x86)", "3.0.100 Preview 5")]
        public void TestDotNetCoreDisplayNameExtractionRegexAccept(string s, string version)
        {
            var match = Regexes.DotNetCoreDisplayNameExtractionRegex.Match(s);

            match.Groups[Regexes.DotNetCoreExtractionRegexVersionGroupName].Value
                .Should().Be(version);
        }

        [Theory]
        [InlineData("Microsoft .NET Core SDK 2.2.300")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.300 (x84)")]
        [InlineData("Microsoft  .NET Core SDK 2.2.300 (x86)")]
        [InlineData("Microsoft .NET  Core SDK 2.2.300 (x86)")]
        [InlineData("Microsoft .NET Core  SDK 2.2.300 (x86)")]
        [InlineData("Microsoft .NET Core SDK  2.2.300 (x86)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300  (x86)")]
        [InlineData(" Microsoft .NET Core SDK 2.2.300 (x86)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x86) ")]
        [InlineData("Hello Microsoft .NET Core SDK 2.2.300 (x86) World")]
        [InlineData("HelloMicrosoft .NET Core SDK 2.2.300 (x86)World")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.0 - preview5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.10 - preview5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.0 - preview 5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime 3.0.0 - preview5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime 3.0.10 - preview5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime 3.0.0 - preview 5 (x64)")]
        public void TestDotNetCoreDisplayNameExtractionRegexReject(string s)
        {
            Regexes.DotNetCoreDisplayNameExtractionRegex.IsMatch(s)
                .Should().BeFalse();
        }
    }
}
