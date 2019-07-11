using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Utils
{
    public class RegexesTests
    {
        private static readonly string TestCachePathPrefex = @"C:\ProgramData\Package Cache\{01234567-89b-cdef-0123-456789abcdef}";

        [Fact]
        internal void TestBundlePublisherRegexAccept()
        {
            TestRegexAccept(Regexes.BundlePublisherRegex, "Microsoft Corporation");
        }

        [Theory]
        [InlineData("")]
        [InlineData("MICROSOFT CORPORATION")]
        [InlineData("microsoft corporation")]
        [InlineData(" Microsoft Corporation")]
        [InlineData("Microsoft Corporation ")]
        [InlineData("Microsoft  Corporation")]
        [InlineData("Microsoft")]
        [InlineData("HelloMicrosoft CorporationWorld")]
        [InlineData("Hello Microsoft Corporation World")]
        internal void TestBundlePublisherRegexReject(string input)
        {
            TestRegexReject(Regexes.BundlePublisherRegex, input);
        }

        [Theory]
        [InlineData("1.0")]
        [InlineData("1.1")]
        [InlineData("2.0")]
        [InlineData("2.1")]
        [InlineData("2.2")]
        [InlineData("3.0")]
        [InlineData("12.345")]
        [InlineData("0012.00345")]
        internal void TestBundleMajorMinorRegexAccept(string input)
        {
            TestRegexAccept(Regexes.BundleMajorMinorRegex, input);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2")]
        [InlineData("2.")]
        [InlineData("2.2.")]
        [InlineData("2.2.2")]
        [InlineData("2.2.202")]
        [InlineData("a.0")]
        [InlineData("0.a")]
        [InlineData("2.2-preview")]
        [InlineData("2.2-preview-011768")]
        [InlineData("2.2-preview-011768-15")]
        [InlineData("3.0.0-preview5-27626-15")]
        [InlineData("3.0.100-preview5-011568")]
        [InlineData("Hello2.2World")]
        [InlineData("Hello 2.2 World")]
        [InlineData("2. 2")]
        [InlineData("2 .2")]
        [InlineData("2 . 2")]
        internal void TestBundleMajorMinorRegexReject(string input)
        {
            TestRegexReject(Regexes.BundleMajorMinorRegex, input);
        }

        [Theory]
        [InlineData("Microsoft .NET Core SDK 2.2.202 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.1.300 - rc1 (x86)")]
        [InlineData("Microsoft .NET Core SDK 3.0.100 - preview5 (x64)")]
        [InlineData(".NET Core SDK 1.1.10 (x64)")]
        [InlineData("Microsoft .NET Core SDK - 2.1.4 (x86)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.4 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 2.1.0 Release Candidate 1 (x86)")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.0 Preview 5 (x64)")]
        [InlineData("Microsoft .NET Core 1.1.13 - Runtime (x64)")]
        internal void TestBundleDisplayNameRegexAccept(string input)
        {
            TestRegexAccept(Regexes.BundleDisplayNameRegex, input);
        }

        [Theory]
        [InlineData(".NET Core SDK - 1.1.10 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.1.300 - Release Candidate 1 (x64)")]
        [InlineData("Microsoft .NET Core SDK 3.0.100 - Preview 5 (x86)")]
        [InlineData("Microsoft .NET Core SDK - 2.1.300 Release Candidate 1 (x64)")]
        [InlineData("Microsoft .NET Core SDK - 3.0.100 Preview 5 (x86)")]
        [InlineData("Microsoft .NET Core Runtime 2.2.4 (x64)")]
        [InlineData("Microsoft .NET Core Runtime 2.1.0 rc1 (x86)")]
        [InlineData("Microsoft .NET Core Runtime 3.0.0 preview5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime 2.1.0 - rc1 (x86)")]
        [InlineData("Microsoft .NET Core Runtime 3.0.0 - preview5 (x64)")]
        [InlineData(".NET Core Runtime - 2.2.4 (x64)")]
        [InlineData(".NET Core Runtime - 2.1.0 Release Candidate 1 (x86)")]
        [InlineData(".NET Core Runtime - 3.0.0 Preview 5 (x64)")]
        [InlineData("HelloMicrosoft .NET Core SDK 2.2.202 (x64)World")]
        [InlineData("Hello Microsoft .NET Core SDK 2.2.202 (x64) World")]
        [InlineData("Microsoft .NET Core SDK 2.2.202 (x66)")]
        [InlineData("Microsoft .NET Core SDK 2.2.202")]
        [InlineData(" Microsoft .NET Core SDK 2.2.202 (x64)")]
        [InlineData("Microsoft  .NET Core SDK 2.2.202 (x64)")]
        [InlineData("Microsoft .NET  Core SDK 2.2.202 (x64)")]
        [InlineData("Microsoft .NET Core  SDK 2.2.202 (x64)")]
        [InlineData("Microsoft .NET Core SDK  2.2.202 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.2.202  (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.2.202 (x64) ")]
        internal void TestBundleDisplayNameRegexReject(string input)
        {
            TestRegexReject(Regexes.BundleDisplayNameRegex, input);
        }

        [Theory]
        [InlineData(@"\dotnet-sdk-2.1.4-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.700-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc1-008673-win-x86.exe")]
        [InlineData(@"\dotnet-sdk-3.0.100-preview5-0011568-win-x86.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc1-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.2.5-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-3.0.0-preview5-27626-15-win-x86.exe")]
        [InlineData(@"\dotnet-dev-win-x64.1.1.14.exe")]
        [InlineData(@"\dotnet-win-x64.1.0.11.exe")]
        internal void TestBundleCachePathRegexAccept(string suffix)
        {
            TestRegexAccept(Regexes.BundleCachePathRegex, $"{TestCachePathPrefex}{suffix}");
        }

        [Theory]
        [InlineData(@"\dotnet-sdk-2.1.300-preview-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-preview1-win-x86.exe")]
        [InlineData(@"\dotnet-sdk-2.0.300-preview1-008174-01-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc1-win-x86.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc1-002111-01-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc1-final-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.0.0-preview-win-x86.exe")]
        [InlineData(@"\dotnet-runtime-2.0.0-preview1-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.0.0-preview1-002111-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc-win-x86.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc1-002111-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc1-002111-01-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc1-final-win-x86.exe")]
        [InlineData(@"\dotnet-dev-win-x64-1.1.14.exe")]
        [InlineData(@"\dotnet-win-x64-1.0.11.exe")]
        [InlineData(@"\dotnet-dev-win-x64.3.0.0-preview5-27626-15.exe")]
        [InlineData(@"\dotnet-win-x64.2.1.300-preview1-008174.exe")]
        [InlineData(@"\dotnet-sdk-2.1.700.exe")]
        [InlineData(@"\dotnet-sdk-2.1.700-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.700-win.exe")]
        [InlineData(@"\dotnet-sdk-2.1.700-windows-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.4-win-x66.exe")]
        [InlineData(@"\dotnet-sdk-2.1.4-win-x64.exeHelloWorld")]
        [InlineData(@"\dotnet-sdk-2.1.4-win-x64.exe Hello World")]
        internal void TestBundleCachePathRegexReject(string suffix)
        {
            TestRegexReject(Regexes.BundleCachePathRegex, $"{TestCachePathPrefex}{suffix}");
        }

        private void TestRegexAccept(Regex regex, string input)
        {
            regex.IsMatch(input).Should().BeTrue();
        }

        private void TestRegexReject(Regex regex, string input)
        {
            regex.IsMatch(input).Should().BeFalse();
        }
    }
}
