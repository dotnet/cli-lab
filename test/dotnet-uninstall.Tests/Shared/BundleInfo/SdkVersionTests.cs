using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
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
    }
}
