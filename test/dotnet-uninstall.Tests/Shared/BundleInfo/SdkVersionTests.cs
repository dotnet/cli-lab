using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class SdkVersionTests
    {
        [Theory]
        [InlineData(2, 2, 3, 0, 32768, false, "2.2.300")]
        [InlineData(0, 2, 3, 0, 32768, false, "0.2.300")]
        [InlineData(2, 0, 3, 0, 32768, false, "2.0.300")]
        [InlineData(2, 2, 0, 2, 32768, false, "2.2.022")]
        [InlineData(3, 0, 1, 0, 11568, true, "3.0.100-preview5-011568")]
        internal void TestConstructor(int major, int minor, int sdkMinor, int patch, int build, bool preview, string displayVersion)
        {
            var version = new SdkVersion(major, minor, sdkMinor, patch, build, preview, displayVersion);

            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.SdkMinor.Should().Be(sdkMinor);
            version.Patch.Should().Be(patch);
            version.Build.Should().Be(build);
            version.Preview.Should().Be(preview);
            version.Type.Should().Be(BundleType.Sdk);
            version.ToString().Should().Be(displayVersion);
        }

        [Theory]
        [InlineData(-2, 2, 3, 0, 32768, false, "-2.2.300")]
        [InlineData(-1, 2, 3, 0, 32768, false, "-1.2.300")]
        [InlineData(3, -1, 1, 0, 11568, true, "3.-1.100-preview5-011568")]
        [InlineData(3, 0, -1, 0, 11568, true, "3.1.-100-preview5-011568")]
        [InlineData(3, 0, 1, -1, 11568, true, "3.1.-1-1-preview5-011568")]
        internal void TestConstructorArgumentOutOfRangeException(int major, int minor, int sdkMinor, int patch, int build, bool preview, string displayVersion)
        {
            Action action = () => new SdkVersion(major, minor, sdkMinor, patch, build, preview, displayVersion);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(2, 2, 3, 0, 32768, false, null)]
        [InlineData(3, 0, 1, 0, 11568, true, null)]
        internal void TestConstructorArgumentNullException(int major, int minor, int sdkMinor, int patch, int build, bool preview, string displayVersion)
        {
            Action action = () => new SdkVersion(major, minor, sdkMinor, patch, build, preview, displayVersion);
            action.Should().Throw<ArgumentNullException>();
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 0, 32768, false, "2.2.300"),
                new SdkVersion(2, 2, 3, 0, 32768, false, "2.2.300")
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, 11568, true, "3.0.100-preview5-011568"),
                new SdkVersion(3, 0, 1, 0, 11568, true, "3.0.100-preview5-011568")
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, 11568, false, "3.0.100-preview5-011568"),
                new SdkVersion(3, 0, 1, 0, 11568, true, "3.0.100-preview5-011568")
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, 11568, true, "3.0.100-preview5-011568"),
                new SdkVersion(3, 0, 1, 0, 11568, true, "some random display version")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestEquality))]
        internal void TestEquality(SdkVersion version1, SdkVersion version2)
        {
            version1.Equals((object)version2).Should().BeTrue();
            version1.CompareTo((object)version2).Should().Be(0);

            version2.Equals((object)version1).Should().BeTrue();
            version2.CompareTo((object)version1).Should().Be(0);
        }

        public static IEnumerable<object[]> GetDataForTestInequality()
        {
            yield return new object[]
            {
                new SdkVersion(1, 2, 3, 0, 32768, false, "1.2.300"),
                new SdkVersion(2, 2, 3, 0, 32768, false, "2.2.300")
            };

            yield return new object[]
            {
                new SdkVersion(2, 1, 3, 0, 32768, false, "2.1.300"),
                new SdkVersion(2, 2, 3, 0, 32768, false, "2.2.300")
            };

            yield return new object[]
            {
                new SdkVersion(2, 2, 2, 0, 32768, false, "2.2.200"),
                new SdkVersion(2, 2, 3, 0, 32768, false, "2.2.300")
            };

            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 2, 32768, false, "2.2.302"),
                new SdkVersion(2, 2, 3, 42, 32768, false, "2.2.342")
            };

            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 0, 32758, true, "2.2.300"),
                new SdkVersion(2, 2, 3, 0, 32768, false, "2.2.300")
            };

            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 0, 32758, false, "2.2.300"),
                new SdkVersion(2, 2, 3, 0, 32768, true, "2.2.300")
            };

            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 0, 32758, false, "some random display version"),
                new SdkVersion(2, 2, 3, 0, 32768, false, "2.2.300")
            };

            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 0, 32758, false, "2.2.300"),
                new SdkVersion(2, 2, 3, 0, 32768, false, "some random display version")
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, 9812, true, "3.0.100-preview-009812"),
                new SdkVersion(3, 0, 1, 0, 11568, true, "3.0.100-preview5-011568"),
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, 9812, true, "3.0.100-preview5-011568"),
                new SdkVersion(3, 0, 1, 0, 11568, true, "3.0.100-preview-009812"),
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequality))]
        internal void TestInequality(SdkVersion version1, SdkVersion version2)
        {
            version1.Equals((object)version2).Should().BeFalse();
            version1.CompareTo((object)version2).Should().BeLessThan(0);

            version2.Equals((object)version1).Should().BeFalse();
            version2.CompareTo((object)version1).Should().BeGreaterThan(0);
        }

        public static IEnumerable<object[]> GetDataForTestInequalityNull()
        {
            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 0, 32768, false, "2.2.300")
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, 11568, true, "3.0.100-preview5-011568")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequalityNull))]
        internal void TestInequalityNull(SdkVersion version)
        {
            version.Equals(null).Should().BeFalse();
            version.CompareTo(null).Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("1.0.0-preview2-003121", 1, 0, 0, 0, 3121, true)]
        [InlineData("1.0.4", 1, 0, 0, 4, 0, false)]
        [InlineData("1.1.14", 1, 1, 0, 14, 0, false)]
        [InlineData("1.0.0-preview2.1-003177", 1, 0, 0, 0, 3177, true)]
        [InlineData("2.0.0-preview1-005977", 2, 0, 0, 0, 5977, true)]
        [InlineData("2.0.0", 2, 0, 0, 0, 0, false)]
        [InlineData("2.1.100", 2, 1, 1, 0, 0, false)]
        [InlineData("2.1.105", 2, 1, 1, 5, 0, false)]
        [InlineData("2.1.202", 2, 1, 2, 2, 0, false)]
        [InlineData("2.1.300-preview1-008174", 2, 1, 3, 0, 8174, true)]
        [InlineData("2.1.300-rc1-008673", 2, 1, 3, 0, 8673, true)]
        [InlineData("2.1.302", 2, 1, 3, 2, 0, false)]
        [InlineData("2.2.300", 2, 2, 3, 0, 0, false)]
        [InlineData("3.0.100-preview-009812", 3, 0, 1, 0, 9812, true)]
        [InlineData("3.0.100-preview6-012264", 3, 0, 1, 0, 12264, true)]
        [InlineData("1.0", 1, 0, 0, 0, 0, false)]
        [InlineData("1.1", 1, 1, 0, 0, 0, false)]
        [InlineData("2.0", 2, 0, 0, 0, 0, false)]
        [InlineData("2.1", 2, 1, 0, 0, 0, false)]
        [InlineData("2.2", 2, 2, 0, 0, 0, false)]
        [InlineData("3.0", 3, 0, 0, 0, 0, false)]
        [InlineData("12.345", 12, 345, 0, 0, 0, false)]
        [InlineData("0012.00345", 12, 345, 0, 0, 0, false)]
        internal void TestFromInputAccept(string input, int major, int minor, int sdkMinor, int patch, int build, bool preview)
        {
            var version = BundleVersion.FromInput<SdkVersion>(input) as SdkVersion;

            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.SdkMinor.Should().Be(sdkMinor);
            version.Patch.Should().Be(patch);
            version.Build.Should().Be(build);
            version.Preview.Should().Be(preview);
            version.ToString().Should().Be(input);
        }

        [Theory]
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
        internal void TestFromInputReject(string input)
        {
            Action action = () => { _ = BundleVersion.FromInput<SdkVersion>(input) as SdkVersion; };
            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
