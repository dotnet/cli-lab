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
        [InlineData("Microsoft ASP.NET Core 3.0.0 Preview 6 Build 19307.2 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.2.6 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.2.0 Preview 3 Build 35497 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.1.0 Release Candidate 1 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.1.0 Preview 2 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.0.9 - Runtime Package Store")]
        [InlineData("Microsoft ASP.NET Core 3.0.0 Preview Build 18579-0056 - Shared Framework")]
        [InlineData("Microsoft .NET Core 1.0.9 & 1.1.6 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 1.0.16 & 1.1.13 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.0.3 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core  Preview 1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core  Release Candidate 1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.1.12 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 1 35029 preview1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 2 35157 35157 preview2 - Windows Server Hosting - 35157")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 3 35497 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.2.0 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.2.6 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview Build 18579-0056 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview Build 19075-0444 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview 4 Build 19216-03 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview 5 Build 19227-01 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview 6 Build 19307.2 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.1.0 Preview 1 Build preview1.19307.20 - Windows Server Hosting")]
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
        [InlineData("Microsoft ASP.NET Core 2.2.6-Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.2.6 - SharedFramework")]
        [InlineData("ASP.NET Core 3.0.0 Preview 6 Build 19307.2 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 3.0.0 Preview 6 Build 19307.2 - Shared Framework (x64)")]
        [InlineData("Microsoft .NET Core 1.0.9&1.1.6 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.0.3-Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.0.3 - WindowsServerHosting")]
        [InlineData("Microsoft .NET Core Preview 1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core Release Candidate 1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core   Preview 1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core   Release Candidate 1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core  Preview1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core  Release Candidate1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview1 35029 preview1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 1-35029 preview1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 1 35029 preview 1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 1 35029 preview1 - Windows Server Hosting-35029")]
        [InlineData("Microsoft .NET Core 2.2.0 preview1 35029 preview1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 1 35029 Preview1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 1 35029 Preview 1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 preview 1 35029 preview1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 2 35157-35157 preview2 - Windows Server Hosting - 35157")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 3-35497 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview Build 18579 0056 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview4 Build 19216-03 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 preview4 Build 19216-03 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 preview 4 Build 19216-03 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview 5 19227-01 - Windows Server Hosting")]
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
        [InlineData(@"\AspNetCore.2.0.9.RuntimePackageStore_x64.exe")]
        [InlineData(@"\aspnetcore-runtime-2.2.6-win-x86.exe")]
        [InlineData(@"\aspnetcore-runtime-2.1.0-preview1-final-win-x64.exe")]
        [InlineData(@"\aspnetcore-runtime-2.1.0-rc1-final-win-x64.exe")]
        [InlineData(@"\aspnetcore-runtime-2.2.0-preview3-35497-win-x64.exe")]
        [InlineData(@"\aspnetcore-runtime-3.0.0-preview-18579-0056-win-x64.exe")]
        [InlineData(@"\aspnetcore-runtime-3.0.0-preview6.19307.2-win-x64.exe")]
        [InlineData(@"\DotNetCore.1.0.9_1.1.6-WindowsHosting.exe")]
        [InlineData(@"\DotNetCore.1.0.10_1.1.7-WindowsHosting.exe")]
        [InlineData(@"\dotnetcore.1.0.14_1.1.11-windowshosting.exe")]
        [InlineData(@"\DotNetCore.2.0.3-WindowsHosting.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-preview1-final-win.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-rc1-final-win.exe")]
        [InlineData(@"\dotnet-hosting-2.1.12-win.exe")]
        [InlineData(@"\dotnet-hosting-2.2.0-preview2-35157-win.exe")]
        [InlineData(@"\dotnet-hosting-2.2.6-win.exe")]
        [InlineData(@"\dotnet-hosting-3.0.0-preview-18579-0056-win.exe")]
        [InlineData(@"\dotnet-hosting-3.0.0-preview5-19227-01-win.exe")]
        [InlineData(@"\dotnet-hosting-3.0.0-preview6.19307.2-win.exe")]
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
        [InlineData(@"\AspNetCore.2.0.9.RuntimePackageStore.exe")]
        [InlineData(@"\AspNetCore.2.0.9.RuntimePackageStore_x66.exe")]
        [InlineData(@"\AspNetCore.2.0.9.RuntimePackageStore.x64.exe")]
        [InlineData(@"\AspNetCore.2.0.9.RuntimePackageStore-x64.exe")]
        [InlineData(@"\aspnetcore-runtime-2.2.6-win.exe")]
        [InlineData(@"\aspnetcore-runtime-2.2.6-win-x66.exe")]
        [InlineData(@"\aspnetcore-runtime-2.1.0-preview1-win-x64.exe")]
        [InlineData(@"\aspnetcore-runtime-2.1.0-rc1-win-x64.exe")]
        [InlineData(@"\DotNetCore.1.0.9_1.1.6-windowshosting.exe")]
        [InlineData(@"\dotnetcore.1.0.9_1.1.6-WindowsHosting.exe")]
        [InlineData(@"\DotNetCore-1.0.9-1.1.6-WindowsHosting.exe")]
        [InlineData(@"\DotNetCore_1.0.9_1.1.6_WindowsHosting.exe")]
        [InlineData(@"\DotNetCore.1.0.9.1.1.6.WindowsHosting.exe")]
        [InlineData(@"\dotnetcore.1.0.14_1.1.11-WindowsHosting.exe")]
        [InlineData(@"\DotNetCore.1.0.14_1.1.11-windowshosting.exe")]
        [InlineData(@"\DotNetCore.2.0.3-windowshosting.exe")]
        [InlineData(@"\dotnetcore.2.0.3-WindowsHosting.exe")]
        [InlineData(@"\DotNetCore-2.0.3-WindowsHosting.exe")]
        [InlineData(@"\DotNetCore_2.0.3_WindowsHosting.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-preview1-win.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-preview1-final.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-preview1-final-win-x64.exe")]
        [InlineData(@"\DotNet-Hosting-2.1.0-Preview1-final-win.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-rc1-win.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-rc1-final.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-rc1-final-win-x64.exe")]
        [InlineData(@"\DotNet-Hosting-2.1.0-RC1-final-win.exe")]
        [InlineData(@"\dotnet-hosting-2.1.0-release-candidate-1-final-win.exe")]
        [InlineData(@"\dotnet-hosting-2.1.12-win-x64.exe")]
        [InlineData(@"\dotnet-hosting-2.2.0-preview2-win.exe")]
        [InlineData(@"\dotnet-hosting-3.0.0-preview6.19307-win.exe")]
        [InlineData(@"\dotnet-hosting-3.0.0-preview6-19307.2-win.exe")]
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
