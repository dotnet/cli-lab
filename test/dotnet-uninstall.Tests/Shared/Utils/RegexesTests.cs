using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Utils
{
    public class RegexesTests
    {
        [Fact]
        public void TestDotNetCoreBundlePublisherRegexAccept()
        {
            Regexes.DotNetCoreBundlePublisherRegex.IsMatch("Microsoft Corporation")
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
        public void TestDotNetCoreBundlePublisherRegexReject(string s)
        {
            Regexes.DotNetCoreBundlePublisherRegex.IsMatch(s)
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
        public void TestDotNetCoreBundleVersionRegexAccept(string s, string major, string minor, string patch, string preview)
        {
            var match = Regexes.DotNetCoreBundleVersionRegex.Match(s);

            match.Groups[Regexes.VersionMajorGroupName].Value
                .Should().Be(major);

            match.Groups[Regexes.VersionMinorGroupName].Value
                .Should().Be(minor);

            match.Groups[Regexes.VersionRegexPatchGroupName].Value
                .Should().Be(patch);

            match.Groups[Regexes.VersionRegexPreviewGroupName].Value
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
        public void TestDotNetCoreBundleVersionRegexReject(string s)
        {
            Regexes.DotNetCoreBundleVersionRegex.IsMatch(s)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("2.2", "2", "2")]
        [InlineData("3.0", "3", "0")]
        [InlineData("03.0", "03", "0")]
        [InlineData("3.00", "3", "00")]
        public void TestDotNetCoreBundleMajorMinorRegexAccept(string s, string major, string minor)
        {
            var match = Regexes.DotNetCoreBundleMajorMinorRegex.Match(s);

            match.Groups[Regexes.VersionMajorGroupName].Value
                .Should().Be(major);

            match.Groups[Regexes.VersionMinorGroupName].Value
                .Should().Be(minor);
        }

        [Theory]
        [InlineData("a.0")]
        [InlineData("0.a")]
        [InlineData("2,2")]
        public void TestDotNetCoreBundleMajorMinorRegexReject(string s)
        {
            Regexes.DotNetCoreBundleMajorMinorRegex.IsMatch(s)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x64)", "2.2.300")]
        [InlineData("Microsoft .NET Core SDK 3.0.100-preview5 (x64)", "3.0.100-preview5")]
        [InlineData("Microsoft .NET Core SDK 3.0.100 - preview5 (x64)", "3.0.100 - preview5")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5 (x86)", "2.2.5")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.0 Preview 5 (x86)", "3.0.0 Preview 5")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.100 Preview 5 (x86)", "3.0.100 Preview 5")]
        public void TestDotNetCoreBundleDisplayNameRegexAccept(string s, string version)
        {
            var match = Regexes.DotNetCoreBundleDisplayNameRegex.Match(s);

            match.Groups[Regexes.VersionRegexVersionGroupName].Value
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
        public void TestDotNetCoreBundleDisplayNameRegexReject(string s)
        {
            Regexes.DotNetCoreBundleDisplayNameRegex.IsMatch(s)
                .Should().BeFalse();
        }
    }
}
