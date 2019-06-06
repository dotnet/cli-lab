using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Windows
{
    public class RegistryManagerTests
    {
        private static readonly string MicrosoftCorporationPublisher = "Microsoft Corporation";
        private static readonly string X64DisplayNameFormat = "Microsoft .NET Core SDK {0}.{1}.{2} (x64)";
        private static readonly string X86DisplayNameFormat = "Microsoft .NET Core SDK {0}.{1}.{2} (x86)";

        [Fact]
        public void TestIsDotNetCoreSdkDisplayName()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X64DisplayNameFormat, 2, 2, 300))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkPublisher()
        {
            RegistryManager.IsDotNetCoreSdkPublisher(MicrosoftCorporationPublisher)
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameNull()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(null)
                .Should().BeFalse();
        }
        
        [Fact]
        public void IsDotNetCoreSdkPublisherNull()
        {
            RegistryManager.IsDotNetCoreSdkPublisher(null)
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameVersion1()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, 1, 23, 456))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameVersion2()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, "a", 23, 456))
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameVersion3()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, 1, "bc", 456))
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameVersion4()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, 1, 23, "def"))
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameVersion5()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, "0", 23, 456))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameVersion6()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, "01", 23, 456))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameVersion7()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, 1, "023", 456))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameVersion8()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, 1, 23, "0456"))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace1()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft  .NET Core SDK 2.2.300 (x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace2()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET   Core SDK 2.2.300 (x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace3()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET Core    SDK 2.2.300 (x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace4()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET Core SDK  2.2.300 (x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace5()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET Core SDK 2.2.300   (x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace6()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(" Microsoft .NET Core SDK 2.2.300 (x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace7()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET Core SDK 2.2.300 (x64) ")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace8()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Micro soft .NET Core SDK 2.2.300 (x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace9()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET Core SDK 2. 2.300 (x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameWhiteSpace10()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET Core SDK 2.2.300 ( x64)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameCaseSensitivity1()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X64DisplayNameFormat, 1, 23, 456).ToUpper())
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNameCaseSensitivity2()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X64DisplayNameFormat, 1, 23, 456).ToLower())
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNamePlatform1()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X64DisplayNameFormat, 1, 23, 456))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNamePlatform2()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName(string.Format(X86DisplayNameFormat, 1, 23, 456))
                .Should().BeTrue();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNamePlatform3()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET Core SDK 1.23.456 (x88)")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkDisplayNamePlatform4()
        {
            RegistryManager.IsDotNetCoreSdkDisplayName("Microsoft .NET Core SDK 1.23.456")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkPublisherIncorrect()
        {
            RegistryManager.IsDotNetCoreSdkPublisher("JetBrains")
                .Should().BeFalse();
        }
            

        [Fact]
        public void TestIsDotNetCoreSdkPublisherWhiteSpace1()
        {
            RegistryManager.IsDotNetCoreSdkPublisher("Microsoft  Corporation")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkPublisherWhiteSpace2()
        {
            RegistryManager.IsDotNetCoreSdkPublisher("  Microsoft Corporation")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkPublisherWhiteSpace3()
        {
            RegistryManager.IsDotNetCoreSdkPublisher("Microsoft Corporation   ")
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkPublisherCaseSensitivity1()
        {
            RegistryManager.IsDotNetCoreSdkPublisher(MicrosoftCorporationPublisher.ToUpper())
                .Should().BeFalse();
        }

        [Fact]
        public void TestIsDotNetCoreSdkPublisherCaseSensitivity2()
        {
            RegistryManager.IsDotNetCoreSdkPublisher(MicrosoftCorporationPublisher.ToLower())
            .Should().BeFalse();
        }
    }
}
