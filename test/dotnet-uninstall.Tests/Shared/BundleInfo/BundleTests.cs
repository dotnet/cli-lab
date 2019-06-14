using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class BundleTests
    {
        private static readonly RuntimeVersion TestVersion = new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5");
        private static readonly string TestUninstallCommand1 = "C:\\ProgramData\\Package Cache\\{a05f1bee-210e-401f-9e98-d52a4698bc91}\\dotnet-sdk-2.2.300-win-x64.exe\" /uninstall /quiet";
        private static readonly string TestUninstallCommand2 = "some random uninstall command";

        [Fact]
        internal void TestConstructor()
        {
            var version = TestVersion;
            var uninstallCommand = TestUninstallCommand1;

            var bundle = new Bundle<RuntimeVersion>(version, BundleArch.X64, uninstallCommand);

            bundle.Version.Should().Be(version);
            bundle.Arch.Should().Be(BundleArch.X64);
            bundle.UninstallCommand.Should().Be(uninstallCommand);
        }

        public static IEnumerable<object[]> GetDataForTestConstructorNull()
        {
            yield return new object[]
            {
                null,
                BundleArch.X64,
                TestUninstallCommand1
            };

            yield return new object[]
            {
                TestVersion,
                BundleArch.X86,
                null
            };

            yield return new object[]
            {
                null,
                BundleArch.Arm32,
                null
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestConstructorNull))]
        internal void TestConstructorNull(RuntimeVersion version, BundleArch arch, string uninstallCommand)
        {
            Action action = () => new Bundle<RuntimeVersion>(version, arch, uninstallCommand);
            action.Should().Throw<ArgumentNullException>();
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new Bundle<RuntimeVersion>(TestVersion, BundleArch.X86, TestUninstallCommand1),
                new Bundle<RuntimeVersion>(TestVersion, BundleArch.X86, TestUninstallCommand1)
            };

            yield return new object[]
            {
                new Bundle<RuntimeVersion>(TestVersion, BundleArch.Arm32, TestUninstallCommand1),
                new Bundle<RuntimeVersion>(TestVersion, BundleArch.Arm32, TestUninstallCommand2)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestEquality))]
        internal void TestEquality(Bundle<RuntimeVersion> bundle1, Bundle<RuntimeVersion> bundle2)
        {
            bundle1.Equals((object)bundle2).Should().BeTrue();
        }

        public static IEnumerable<object[]> GetDataForTestToString()
        {
            yield return new object[]
            {
                TestVersion,
                BundleArch.X64
            };

            yield return new object[]
            {
                TestVersion,
                BundleArch.X86
            };

            yield return new object[]
            {
                TestVersion,
                BundleArch.Arm32
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestToString))]
        internal void TestToString(RuntimeVersion version, BundleArch arch)
        {
            new Bundle<RuntimeVersion>(version, arch, TestUninstallCommand1).ToString()
                .Should().Be($"{version.ToString()} ({arch.ToString().ToLower()})");
        }
    }
}
