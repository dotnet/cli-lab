using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Windows
{
    public class SdkRegistryQueryTests
    {
        private static readonly string MicrosoftCorporationPublisher = "Microsoft Corporation";
        private static readonly string X64SdkDisplayNameFormat = "Microsoft .NET Core SDK {0}.{1}.{2} (x64)";
        private static readonly string X86SdkDisplayNameFormat = "Microsoft .NET Core SDK {0}.{1}.{2} (x86)";
        private static readonly string X64RuntimeDisplayNameFormat = "Microsoft .NET Core Runtime - {0}.{1}.{2} (x64)";
        private static readonly string X86RuntimeDisplayNameFormat = "Microsoft .NET Core Runtime - {0}.{1}.{2} (x86)";

        [Theory]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x86)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5 (x86)")]
        public void TestIsDotNetCoreDisplayNameCorrect(string displayName)
        {
            SdkRegistryQuery.IsDotNetCoreDisplayName(displayName)
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCorePublisherCorrect()
        {
            SdkRegistryQuery.IsDotNetCorePublisher(MicrosoftCorporationPublisher)
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreDisplayNameNull()
        {
            SdkRegistryQuery.IsDotNetCoreDisplayName(null)
                .Should().BeFalse();
        }
        
        [Fact]
        public void IsDotNetCorePublisherNull()
        {
            SdkRegistryQuery.IsDotNetCorePublisher(null)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("0", "23", "456")]
        [InlineData("01", "23", "456")]
        [InlineData("1", "023", "456")]
        [InlineData("1", "23", "0456")]
        [InlineData("0", "00", "000")]
        public void TestIsDotNetCoreDisplayNameVersionCorrect(string majorVersion, string minorVersion, string patchVersion)
        {
            SdkRegistryQuery.IsDotNetCoreDisplayName(string.Format(X86SdkDisplayNameFormat, majorVersion, minorVersion, patchVersion))
                .Should().BeTrue();
            SdkRegistryQuery.IsDotNetCoreDisplayName(string.Format(X86RuntimeDisplayNameFormat, majorVersion, minorVersion, patchVersion))
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("a", "23", "456")]
        [InlineData("1", "bc", "456")]
        [InlineData("1", "23", "def")]
        public void TestIsDotNetCoreDisplayNameVersionWrong(string majorVersion, string minorVersion, string patchVersion)
        {
            SdkRegistryQuery.IsDotNetCoreDisplayName(string.Format(X64SdkDisplayNameFormat, majorVersion, minorVersion, patchVersion))
                .Should().BeFalse();
            SdkRegistryQuery.IsDotNetCoreDisplayName(string.Format(X64RuntimeDisplayNameFormat, majorVersion, minorVersion, patchVersion))
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Microsoft  .NET Core SDK 2.2.300 (x64)")]
        [InlineData("Microsoft .NET   Core SDK 2.2.300 (x64)")]
        [InlineData("Microsoft .NET Core    SDK 2.2.300 (x64)")]
        [InlineData("Microsoft .NET Core SDK  2.2.300 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300   (x64)")]
        [InlineData(" Microsoft .NET Core SDK 2.2.300 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x64) ")]
        [InlineData("Micro soft .NET Core SDK 2.2.300 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2. 2.300 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300 ( x64)")]
        [InlineData("Microsoft  .NET Core Runtime - 2.2.5 (x64)")]
        [InlineData("Microsoft .NET   Core Runtime - 2.2.5 (x64)")]
        [InlineData("Microsoft .NET Core    Runtime - 2.2.5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime  - 2.2.5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime -  2.2.5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5   (x64)")]
        [InlineData(" Microsoft .NET Core Runtime - 2.2.5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5 (x64) ")]
        [InlineData("Micro soft .NET Core Runtime - 2.2.5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 2. 2.5 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5 ( x64)")]
        public void TestIsDotNetCoreDisplayNameWhiteSpace(string displayName)
        {
            SdkRegistryQuery.IsDotNetCoreDisplayName(displayName)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("MICROSOFT .NET CORE SDK 2.2.300 (X64)")]
        [InlineData("microsoft .net core sdk 2.2.300 (x64)")]
        [InlineData("mICROSOFT .net cORE rUNTIME - 2.2.5 (x64)")]
        [InlineData("MiCrOsOfT .nEt CoRe RuNtImE - 2.2.5 (x64)")]
        public void TestIsDotNetCoreDisplayNameCaseSensitivity(string displayName)
        {
            SdkRegistryQuery.IsDotNetCoreDisplayName(displayName)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x66)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5 (x88)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.5")]
        public void TestIsDotNetCoreDisplayNamePlatformWrong(string displayName)
        {
            SdkRegistryQuery.IsDotNetCoreDisplayName(displayName)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Microsoft .NET Core SDK - 2.2.300 (x64)")]
        [InlineData("Microsoft .NET Core Runtime 2.2.5 (x86)")]
        public void TestIsDotNetCoreDisplayNameSdkVsRuntimeWrong(string displayName)
        {
            SdkRegistryQuery.IsDotNetCoreDisplayName(displayName)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Yuchong Pan")]
        [InlineData("Microsoft")]
        public void TestIsDotNetCorePublisherWrong(string publisher)
        {
            SdkRegistryQuery.IsDotNetCorePublisher(publisher)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Microsoft  Corporation")]
        [InlineData("  Microsoft Corporation")]
        [InlineData("Microsoft Corporation   ")]
        public void TestIsDotNetCorePublisherWhiteSpace(string publisher)
        {
            SdkRegistryQuery.IsDotNetCorePublisher(publisher)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("MICROSOFT CORPORATION")]
        [InlineData("microsoft corporation")]
        public void TestIsDotNetCorePublisherCaseSensitivity(string publisher)
        {
            SdkRegistryQuery.IsDotNetCorePublisher(publisher)
                .Should().BeFalse();
        }
    }
}
