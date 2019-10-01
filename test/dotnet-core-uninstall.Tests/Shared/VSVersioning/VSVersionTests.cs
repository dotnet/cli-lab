using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using NuGet.Versioning;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.VSVersioning
{
    public class VSVersionTests
    {
        [Theory]
        [InlineData("0.0.999", "C:\\ProgramData\\Package Cache\\{a05f1bee-210e-401f-9e98-d52a4698bc91}\\dotnet-sdk-2.2.300-win-x64.exe\" /uninstall /quiet", 
            "Microsoft .NET Core SDK 2.2.300 (x64)", true)] // Fake, not included in VSVersions.json
        [InlineData("0.0.0", "C:\\ProgramData\\Package Cache\\{a05f1bee-210e-401f-9e98-d52a4698bc91}\\dotnet-sdk-2.2.300-win-x64.exe\" /uninstall /quiet",
            "Microsoft .NET Core SDK 2.2.300 (x64)", false)] // Fake, included in VSVersions.json
        internal void TestUninstallAllowed(string version, string uninstallCommand, string displayName, bool allowed)
        {
            var sdkVersion = new SdkVersion(version);
            var bundle = new Bundle<SdkVersion>(sdkVersion, BundleArch.X64, uninstallCommand, displayName);

            bundle.Version.Should().Be(sdkVersion);
            bundle.Arch.Should().Be(BundleArch.X64);
            bundle.UninstallCommand.Should().Be(uninstallCommand);
            bundle.DisplayName.Should().Be(displayName);
            bundle.UninstallAllowed.Should().Be(allowed);
        }

        [Theory]
        [InlineData(0, 0, 999, true)] // Fake
        [InlineData(16, 3, 0, true)] // 2019 version
        [InlineData(14, 0, 0, false)] // 2015 version
        internal void TestVSToDotnetVersion(int major, int minor, int patch, bool nullRes)
        {
            var outputNull = VSVersionHelper.VSToDotnetVersion(new SemanticVersion(major, minor, patch)) == null;
            outputNull.Should().Be(nullRes);
        }
    }
}
