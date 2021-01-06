using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Utils
{
    public class RegexesTests
    {
        private static readonly string TestCachePathPrefex = @"C:\ProgramData\Package Cache\{01234567-89b-cdef-0123-456789abcdef}";

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
