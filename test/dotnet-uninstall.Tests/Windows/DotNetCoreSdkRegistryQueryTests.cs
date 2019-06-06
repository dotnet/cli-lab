using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Windows
{
    public class DotNetCoreSdkRegistryQueryTests
    {
        private static readonly string MicrosoftCorporationPublisher = "Microsoft Corporation";
        private static readonly string X64DisplayNameFormat = "Microsoft .NET Core SDK {0}.{1}.{2} (x64)";
        private static readonly string X86DisplayNameFormat = "Microsoft .NET Core SDK {0}.{1}.{2} (x86)";

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameCorrect()
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkDisplayName(string.Format(X64DisplayNameFormat, "2", "2", "300"))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkPublisherCorrect()
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkPublisher(MicrosoftCorporationPublisher)
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameNull()
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkDisplayName(null)
                .Should().BeFalse();
        }
        
        [Fact]
        public void IsDotNetCoreSdkPublisherNull()
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkPublisher(null)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("0", "23", "456")]
        [InlineData("01", "23", "456")]
        [InlineData("1", "023", "456")]
        [InlineData("1", "23", "0456")]
        [InlineData("0", "00", "000")]
        public void TestIsDotNetCoreSdkDisplayNameVersionCorrect(string majorVersion, string minorVersion, string patchVersion)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, majorVersion, minorVersion, patchVersion))
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("a", "23", "456")]
        [InlineData("1", "bc", "456")]
        [InlineData("1", "23", "def")]
        public void TestIsDotNetCoreSdkDisplayNameVersionWrong(string majorVersion, string minorVersion, string patchVersion)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, majorVersion, minorVersion, patchVersion))
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
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace(string displayName)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkDisplayName(displayName)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("MICROSOFT .NET CORE SDK 2.2.300 (X64)")]
        [InlineData("microsoft .net core sdk 2.2.300 (x64)")]
        public void TestIsDotNetCoreSdkDisplayNameCaseSensitivity(string displayName)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkDisplayName(displayName)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x86)")]
        public void TestIsDotNetCoreSdkDisplayNamePlatformCorrect(string displayName)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkDisplayName(displayName)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("Microsoft .NET Core SDK 2.2.300 (x66)")]
        [InlineData("Microsoft .NET Core SDK 2.2.300")]
        public void TestIsDotNetCoreSdkDisplayNamePlatformWrong(string displayName)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkDisplayName(displayName)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Yuchong Pan")]
        [InlineData("Microsoft")]
        public void TestIsDotNetCoreSdkPublisherWrong(string publisher)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkPublisher(publisher)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("Microsoft  Corporation")]
        [InlineData("  Microsoft Corporation")]
        [InlineData("Microsoft Corporation   ")]
        public void TestIsDotNetCoreSdkPublisherWhiteSpace(string publisher)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkPublisher(publisher)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("MICROSOFT CORPORATION")]
        [InlineData("microsoft corporation")]
        public void TestIsDotNetCoreSdkPublisherCaseSensitivity(string publisher)
        {
            DotNetCoreSdkRegistryQuery.IsDotNetCoreSdkPublisher(publisher)
                .Should().BeFalse();
        }
    }
}
