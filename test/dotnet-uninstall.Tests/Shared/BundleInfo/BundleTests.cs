using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class BundleTests
    {
        [Fact]
        internal void TestConstructor()
        {
            var version = new RuntimeVersion(2, 2, 5, null);
            var uninstallCommand = "C:\\ProgramData\\Package Cache\\{a05f1bee-210e-401f-9e98-d52a4698bc91}\\dotnet-sdk-2.2.300-win-x64.exe\" /uninstall /quiet";

            var bundle = new Bundle(version, BundleArch.X64, uninstallCommand);

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
                "some random uninstall command"
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5, null),
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
        internal void TestConstructorNull(BundleVersion version, BundleArch arch, string uninstallCommand)
        {
            Action action = () => new Bundle(version, arch, uninstallCommand);
            action.Should().Throw<ArgumentNullException>();
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new Bundle(new SdkVersion(2, 2, 3, 0, null), BundleArch.X86, "same uninstall command"),
                new Bundle(new SdkVersion(2, 2, 3, 0, null), BundleArch.X86, "same uninstall command")
            };

            yield return new object[]
            {
                new Bundle(new RuntimeVersion(2, 2, 5, new PreviewVersion(5, 11736)), BundleArch.Arm32, "some uninstall command"),
                new Bundle(new RuntimeVersion(2, 2, 5, new PreviewVersion(5, 11736)), BundleArch.Arm32, "yet another uninstall command")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestEquality))]
        internal void TestEquality(Bundle bundle1, Bundle bundle2)
        {
            bundle1.Equals((object)bundle2).Should().BeTrue();
            bundle1.CompareTo((object)bundle2).Should().Be(0);
        }

        public static IEnumerable<object[]> GetDataForTestInequality()
        {
            yield return new object[]
            {
                new Bundle(new SdkVersion(2, 1, 7, 0, null), BundleArch.X64, "some uninstall command"),
                new Bundle(new SdkVersion(2, 2, 3, 0, null), BundleArch.X64, "some other uninstall command")
            };

            yield return new object[]
            {
                new Bundle(new SdkVersion(3, 0, 1, 0, new PreviewVersion(5, 12345)), BundleArch.X86, "some uninstall command"),
                new Bundle(new SdkVersion(3, 0, 1, 0, new PreviewVersion(5, 23767)), BundleArch.X86, "some other uninstall command")
            };

            yield return new object[]
            {
                new Bundle(new RuntimeVersion(2, 1, 6, null), BundleArch.Arm32, "same uninstall command"),
                new Bundle(new RuntimeVersion(2, 1, 6, null), BundleArch.X86, "same uninstall command")
            };

            yield return new object[]
            {
                new Bundle(new RuntimeVersion(2, 1, 6, null), BundleArch.Arm32, "same uninstall command"),
                new Bundle(new RuntimeVersion(2, 1, 6, null), BundleArch.X64, "same uninstall command")
            };

            yield return new object[]
            {
                new Bundle(new RuntimeVersion(3, 0, 0, new PreviewVersion(5, 11736)), BundleArch.X86, "same uninstall command"),
                new Bundle(new RuntimeVersion(3, 0, 0, new PreviewVersion(5, 11736)), BundleArch.X64, "same uninstall command")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequality))]
        internal void TestInequality(Bundle bundle1, Bundle bundle2)
        {
            bundle1.Equals((object)bundle2).Should().BeFalse();
            bundle1.CompareTo((object)bundle2).Should().BeLessThan(0);

            bundle2.Equals((object)bundle1).Should().BeFalse();
            bundle2.CompareTo((object)bundle1).Should().BeGreaterThan(0);
        }

        public static IEnumerable<object[]> GetDataForTestToString()
        {
            yield return new object[]
            {
                new SdkVersion(2, 1, 7, 0, null),
                BundleArch.X64
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5, null),
                BundleArch.X86
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new PreviewVersion(5, 23767)),
                BundleArch.Arm32
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestToString))]
        internal void TestToString(BundleVersion version, BundleArch arch)
        {
            new Bundle(version, arch, "some random uninstall command").ToString()
                .Should().Be($"{version.ToString()} ({arch.ToString()})");
        }
    }
}
