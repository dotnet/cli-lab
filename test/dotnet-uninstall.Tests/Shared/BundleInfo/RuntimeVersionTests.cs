using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class RuntimeVersionTests
    {
        [Theory]
        [InlineData(2, 2, 5, 32768, false, "2.2.5")]
        [InlineData(0, 2, 5, 32768, false, "0.2.5")]
        [InlineData(3, 0, 0, 27626, true, "3.0.0-preview5-27626-15")]
        internal void TestConstructor(int major, int minor, int patch, int build, bool preview, string displayVersion)
        {
            var version = new RuntimeVersion(major, minor, patch, build, preview, displayVersion);

            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.Build.Should().Be(build);
            version.Preview.Should().Be(preview);
            version.Type.Should().Be(BundleType.Runtime);
            version.ToString().Should().Be(displayVersion);
        }

        [Theory]
        [InlineData(-2, 2, 5, 32768, false, "-2.2.5")]
        [InlineData(-1, 2, 5, 32768, false, "-1.2.5")]
        [InlineData(3, -1, 0, 32768, false, "3.-1.0")]
        [InlineData(3, 0, -1, 27626, true, "3.0.-1-preview5-27626-15")]
        [InlineData(2, 2, 5, -1, false, "2.2.5")]
        internal void TestConstructorArgumentOutOfRangeException(int major, int minor, int patch, int build, bool preview, string displayVersion)
        {
            Action action = () => new RuntimeVersion(major, minor, patch, build, preview, displayVersion);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(2, 2, 5, 32768, false, null)]
        [InlineData(3, 0, 0, 27626, true, null)]
        internal void TestConstructorArgumentNullException(int major, int minor, int patch, int build, bool preview, string displayVersion)
        {
            Action action = () => new RuntimeVersion(major, minor, patch, build, preview, displayVersion);
            action.Should().Throw<ArgumentNullException>();
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5"),
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, 27626, true, "3.0.0-preview5-27626-15"),
                new RuntimeVersion(3, 0, 0, 27626, true, "3.0.0-preview5-27626-15")
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, 27626, false, "3.0.0-preview5-27626-15"),
                new RuntimeVersion(3, 0, 0, 27626, true, "3.0.0-preview5-27626-15")
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, 27626, true, "3.0.0-preview5-27626-15"),
                new RuntimeVersion(3, 0, 0, 27626, true, "some random display version")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestEquality))]
        internal void TestEquality(RuntimeVersion version1, RuntimeVersion version2)
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
                new RuntimeVersion(1, 2, 5, 32768, false, "1.2.5"),
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 1, 5, 32768, false, "2.1.5"),
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 4, 32768, false, "2.2.4"),
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5, 32758, false, "2.2.5"),
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5, 32758, true, "2.2.5"),
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5, 32758, false, "2.2.5"),
                new RuntimeVersion(2, 2, 5, 32768, true, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5, 32758, false, "some random display version"),
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5, 32758, false, "2.2.5"),
                new RuntimeVersion(2, 2, 5, 32768, false, "some random display version")
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, 27122, true, "3.0.0-preview5-27122-01"),
                new RuntimeVersion(3, 0, 0, 27626, true, "3.0.0-preview5-27626-15")
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, 27122, true, "3.0.0-preview5-27626-15"),
                new RuntimeVersion(3, 0, 0, 27626, true, "3.0.0-preview5-27122-01")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequality))]
        internal void TestInequality(RuntimeVersion version1, RuntimeVersion version2)
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
                new RuntimeVersion(2, 2, 5, 32768, false, "2.2.5")
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, 27626, true, "3.0.0-preview5-27626-15")
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequalityNull))]
        internal void TestInequalityNull(RuntimeVersion version)
        {
            version.Equals(null).Should().BeFalse();
            version.CompareTo(null).Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("1.0.0", 1, 0, 0, 0, false)]
        [InlineData("1.0.16", 1, 0, 16, 0, false)]
        [InlineData("2.0.0-preview1-002111-00", 2, 0, 0, 2111, true)]
        [InlineData("2.1.0-rc1", 2, 1, 0, 0, true)]
        [InlineData("2.2.5", 2, 2, 5, 0, false)]
        [InlineData("3.0.0-preview-27122-01", 3, 0, 0, 27122, true)]
        [InlineData("3.0.0-preview5-27626-15", 3, 0, 0, 27626, true)]
        internal void TestFromInputAccept(string input, int major, int minor, int patch, int build, bool preview)
        {
            var version = BundleVersion.FromInput<RuntimeVersion>(input) as RuntimeVersion;

            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.Build.Should().Be(build);
            version.Preview.Should().Be(preview);
            version.ToString().Should().Be(input);
        }

        [Theory]
        [InlineData("1.0")]
        [InlineData("1.0.")]
        [InlineData("2.0.0-preview")]
        [InlineData("2.0.0-preview1")]
        [InlineData("2.0.0-preview1-002111")]
        [InlineData("2.1.0-rc")]
        [InlineData("2.1.0-rc1-002111")]
        [InlineData("2.1.0-rc1-002111-01")]
        [InlineData("2.1.0-rc1-final")]
        [InlineData("2.2.5.002111")]
        [InlineData("a.0.0")]
        [InlineData("0.a.0")]
        [InlineData("0.0.a")]
        [InlineData("2.0.0-preview1-abcdef-01")]
        [InlineData("2.0.0-preview1-002111-ab")]
        [InlineData("Hello2.2.5World")]
        [InlineData("Hello 2.2.5 World")]
        internal void TestFromInputReject(string input)
        {
            Action action = () => { _ = BundleVersion.FromInput<RuntimeVersion>(input) as RuntimeVersion; };
            action.Should().Throw<InvalidInputVersionException>(string.Format(Messages.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
