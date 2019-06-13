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
        public static IEnumerable<object[]> GetDataForTestConstructor()
        {
            yield return new object[]
            {
                2, 2, 5, null
            };

            yield return new object[]
            {
                3, 0, 0,
                new BundleVersion.PreviewVersion(5, 27363)
            };

            yield return new object[]
            {
                3, 0, 0,
                new BundleVersion.PreviewVersion(null, 27363)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestConstructor))]
        internal void TestConstructor(int major, int minor, int patch, BundleVersion.PreviewVersion preview)
        {
            var version = new RuntimeVersion(major, minor, patch, preview);

            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.Preview.Should().Be(preview);
            version.Type.Should().Be(BundleType.Runtime);
        }

        public static IEnumerable<object[]> GetDataForTestFromInput()
        {
            yield return new object[]
            {
                "2.2.5",
                new RuntimeVersion(2, 2, 5)
            };

            yield return new object[]
            {
                "3.0.0-preview5-27626",
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 27626))
            };

            yield return new object[]
            {
                "3.0.0-preview-27626",
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(null, 27626))
            };

            yield return new object[]
            {
                "3.0.0-preview5-027626",
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 27626))
            };

            yield return new object[]
            {
                "12.345.678-preview9101-11213",
                new RuntimeVersion(12, 345, 678, new BundleVersion.PreviewVersion(9101, 11213))
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFromInput))]
        internal void TestFromInputAccept(string input, RuntimeVersion expected)
        {
            RuntimeVersion.FromInput(input)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2.2")]
        [InlineData("2.2.")]
        [InlineData("a.0.0")]
        [InlineData("0.a.0")]
        [InlineData("0.0.a")]
        [InlineData("3.0.0-preview5")]
        [InlineData("3.0.0 - preview5-32768")]
        [InlineData("3.0.0-preview 5-32768")]
        [InlineData("3.0.0-preview-5-32768")]
        [InlineData("3.0.0-preview5 - 32768")]
        [InlineData("3.0.0 Preview 5-32768")]
        [InlineData("3.0.0 Preview 5 - 32768")]
        [InlineData("3.0.0-previewa-32768")]
        [InlineData("3.0.0-preview-a-32768")]
        [InlineData("3.0.0-preview5-abcde")]
        internal void TestFromInputReject(string input)
        {
            Action action = () => RuntimeVersion.FromInput(input);
            action.Should().Throw<InvalidInputVersionStringException>(
                string.Format(Messages.InvalidInputVersionStringExceptionMessageFormat, input));
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5),
                new RuntimeVersion(2, 2, 5)
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 32768)),
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 32768))
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(null, 32768)),
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(null, 32768))
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestEquality))]
        internal void TestEquality(BundleVersion version1, BundleVersion version2)
        {
            version1.Equals((object)version2).Should().BeTrue();
            version1.CompareTo((object)version2).Should().Be(0);
        }

        public static IEnumerable<object[]> GetDataForTestInequality()
        {
            yield return new object[]
            {
                new RuntimeVersion(1, 2, 5),
                new RuntimeVersion(2, 2, 5)
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 1, 5),
                new RuntimeVersion(2, 2, 5)
            };

            yield return new object[]
            {
                new RuntimeVersion(2, 2, 4),
                new RuntimeVersion(2, 2, 5)
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(4, 32768)),
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 32768))
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 32758)),
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 32768))
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(null, 32758)),
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(null, 32768))
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 32768)),
                new RuntimeVersion(3, 0, 0)
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(null, 32768)),
                new RuntimeVersion(3, 0, 0)
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0),
                new RuntimeVersion(2, 2, 5)
            };

            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 0),
                new RuntimeVersion(2, 2, 5)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequality))]
        internal void TestInequality(BundleVersion version1, BundleVersion version2)
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
                new RuntimeVersion(2, 2, 5)
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 32768)),
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(null, 32768)),
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequalityNull))]
        internal void TestInequalityNull(RuntimeVersion version)
        {
            version.Equals(null).Should().BeFalse();
            version.CompareTo(null).Should().BeGreaterThan(0);
        }

        public static IEnumerable<object[]> GetDataForTestToString()
        {
            yield return new object[]
            {
                new RuntimeVersion(2, 2, 5),
                "2.2.5"
            };

            yield return new object[]
            {
                new RuntimeVersion(12, 34, 56),
                "12.34.56"
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 32768)),
                "3.0.0-preview5-32768"
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(null, 32768)),
                "3.0.0-preview-32768"
            };

            yield return new object[]
            {
                new RuntimeVersion(3, 0, 0, new BundleVersion.PreviewVersion(5, 1024)),
                "3.0.0-preview5-01024"
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestToString))]
        internal void TestToString(RuntimeVersion version, string expected)
        {
            version.ToString().Should().Be(expected);
        }
    }
}
