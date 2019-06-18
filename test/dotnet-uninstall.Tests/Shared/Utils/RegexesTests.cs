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
        [InlineData("Microsoft .NET Core SDK 3.0.100 - preview5 (arm32)")]
        [InlineData(".NET Core SDK 1.1.10 (x64)")]
        [InlineData("Microsoft .NET Core SDK - 2.1.4 (x86)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.4 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 2.1.0 Release Candidate 1 (x86)")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.0 Preview 5 (arm32)")]
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
        [InlineData("Microsoft .NET Core Runtime 3.0.0 preview5 (arm32)")]
        [InlineData("Microsoft .NET Core Runtime 2.1.0 - rc1 (x86)")]
        [InlineData("Microsoft .NET Core Runtime 3.0.0 - preview5 (arm32)")]
        [InlineData(".NET Core Runtime - 2.2.4 (x64)")]
        [InlineData(".NET Core Runtime - 2.1.0 Release Candidate 1 (x86)")]
        [InlineData(".NET Core Runtime - 3.0.0 Preview 5 (arm32)")]
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
        [InlineData("1.1.10.0")]
        [InlineData("2.1.300.8673")]
        [InlineData("3.0.0.27626")]
        [InlineData("12.345.6789.101112")]
        [InlineData("0012.00345.006789.00101112")]
        internal void TestBundleVersionRegexAccept(string input)
        {
            TestRegexAccept(Regexes.BundleVersionRegex, input);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2")]
        [InlineData("2.2")]
        [InlineData("2.2.2")]
        [InlineData("2.2.2.")]
        [InlineData("2.2.2.2.")]
        [InlineData("3.0.0.5.27626")]
        [InlineData("a.0.0.0")]
        [InlineData("0.a.0.0")]
        [InlineData("0.0.a.0")]
        [InlineData("0.0.0.a")]
        [InlineData("2.2.2.2-preview")]
        [InlineData("2.2.2.2-preview-011768")]
        [InlineData("2.2.2.2-preview-011768-15")]
        [InlineData("3.0.0.27626-preview5-27626-15")]
        [InlineData("3.0.100.27626-preview5-011568")]
        [InlineData("Hello2.2.2.2World")]
        [InlineData("Hello 2.2.2.2 World")]
        [InlineData("2 . 2 . 2 . 2")]
        internal void TestBundleVersionRegexReject(string input)
        {
            TestRegexReject(Regexes.BundleVersionRegex, input);
        }

        [Theory]
        [InlineData(@"\dotnet-sdk-2.1.4-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.700-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc1-008673-win-x86.exe")]
        [InlineData(@"\dotnet-sdk-3.0.100-preview5-0011568-win-x86.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc1-win-arm32.exe")]
        [InlineData(@"\dotnet-runtime-2.2.5-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-3.0.0-preview5-27626-15-win-x86.exe")]
        [InlineData(@"\dotnet-dev-win-arm32.1.1.14.exe")]
        [InlineData(@"\dotnet-win-x64.1.0.11.exe")]
        internal void TestBundleCachePathRegexAccept(string suffix)
        {
            TestRegexAccept(Regexes.BundleCachePathRegex, $"{TestCachePathPrefex}{suffix}");
        }

        [Theory]
        [InlineData(@"\dotnet-sdk-2.1.300-preview-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-preview1-win-x86.exe")]
        [InlineData(@"\dotnet-sdk-2.0.300-preview1-008174-01-win-arm32.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc-win-x64.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc1-win-x86.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc1-002111-01-win-arm32.exe")]
        [InlineData(@"\dotnet-sdk-2.1.300-rc1-final-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.0.0-preview-win-x86.exe")]
        [InlineData(@"\dotnet-runtime-2.0.0-preview1-win-arm32.exe")]
        [InlineData(@"\dotnet-runtime-2.0.0-preview1-002111-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc-win-x86.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc1-002111-win-arm32.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc1-002111-01-win-x64.exe")]
        [InlineData(@"\dotnet-runtime-2.1.0-rc1-final-win-x86.exe")]
        [InlineData(@"\dotnet-dev-win-arm32-1.1.14.exe")]
        [InlineData(@"\dotnet-win-x64-1.0.11.exe")]
        [InlineData(@"\dotnet-dev-win-arm32.3.0.0-preview5-27626-15.exe")]
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

        [Theory]
        [InlineData("1.0.0-preview2-003121")]
        [InlineData("1.0.4")]
        [InlineData("1.1.14")]
        [InlineData("1.0.0-preview2.1-003177")]
        [InlineData("2.0.0-preview1-005977")]
        [InlineData("2.0.0")]
        [InlineData("2.1.100")]
        [InlineData("2.1.105")]
        [InlineData("2.1.202")]
        [InlineData("2.1.300-preview1-008174")]
        [InlineData("2.1.300-rc1-008673")]
        [InlineData("2.1.302")]
        [InlineData("2.2.300")]
        [InlineData("3.0.100-preview-009812")]
        [InlineData("3.0.100-preview6-012264")]
        internal void TestSdkVersionInputRegexAccept(string input)
        {
            TestRegexAccept(Regexes.SdkVersionInputRegex, input);
        }

        [Theory]
        [InlineData("1.0")]
        [InlineData("1.0.")]
        [InlineData("2.2.5.002111")]
        [InlineData("2.2.500.002111")]
        [InlineData("2.0.0-preview")]
        [InlineData("2.0.0-preview1")]
        [InlineData("2.0.0-preview1-008174-01")]
        [InlineData("2.1.300-preview")]
        [InlineData("2.1.300-preview1")]
        [InlineData("2.0.300-preview1-008174-01")]
        [InlineData("2.1.300-rc")]
        [InlineData("2.1.300-rc1")]
        [InlineData("2.1.300-rc1-002111-01")]
        [InlineData("2.1.300-rc1-final")]
        [InlineData("a.0.100")]
        [InlineData("0.a.302")]
        [InlineData("0.0.abc")]
        [InlineData("2.0.100-preview1-abcdef")]
        [InlineData("Hello2.2.300World")]
        [InlineData("Hello 2.2.300 World")]
        internal void TestSdkVersionInputRegexReject(string input)
        {
            TestRegexReject(Regexes.SdkVersionInputRegex, input);
        }

        [Theory]
        [InlineData("1.0.0")]
        [InlineData("1.0.16")]
        [InlineData("2.0.0-preview1-002111-00")]
        [InlineData("2.1.0-rc1")]
        [InlineData("2.2.5")]
        [InlineData("3.0.0-preview-27122-01")]
        [InlineData("3.0.0-preview5-27626-15")]
        internal void TestRuntimeVersionInputRegexAccept(string input)
        {
            TestRegexAccept(Regexes.RuntimeVersionInputRegex, input);
        }

        [Theory]
        [InlineData("1.0")]
        [InlineData("1.0.")]
        [InlineData("2.0.0-preview")]
        [InlineData("2.0.0-preview1")]
        [InlineData("2.0.0-preview1-002111")]
        [InlineData("2.1.0-rc")]
        [InlineData("2.1.0-rc1-002111")]
        [InlineData("2.1.0-rc1-002111-01")]
        [InlineData("2.1.0-rc1-final")]
        [InlineData("2.2.5.002111")]
        [InlineData("a.0.0")]
        [InlineData("0.a.0")]
        [InlineData("0.0.a")]
        [InlineData("2.0.0-preview1-abcdef-01")]
        [InlineData("2.0.0-preview1-002111-ab")]
        [InlineData("Hello2.2.5World")]
        [InlineData("Hello 2.2.5 World")]
        internal void TestRuntimeVersionInputRegexReject(string input)
        {
            TestRegexReject(Regexes.RuntimeVersionInputRegex, input);
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
