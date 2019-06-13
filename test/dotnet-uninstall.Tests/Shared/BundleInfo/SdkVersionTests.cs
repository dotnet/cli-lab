using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class SdkVersionTests
    {
        public static IEnumerable<object[]> GetDataForTestConstructor()
        {
            yield return new object[]
            {
                2, 2, 3, 0, null
            };

            yield return new object[]
            {
                0, 2, 3, 0, null
            };

            yield return new object[]
            {
                3, 0, 1, 0,
                new BundleVersion.PreviewVersion(5, 32768)
            };

            yield return new object[]
            {
                3, 0, 1, 0,
                new BundleVersion.PreviewVersion(null, 32768)
            };

            yield return new object[]
            {
                3, 0, 0, 1,
                new BundleVersion.PreviewVersion(null, 32768)
            };

            yield return new object[]
            {
                2, 2, 3, 99, null
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestConstructor))]
        internal void TestConstructor(int major, int minor, int sdkMinor, int patch, BundleVersion.PreviewVersion preview)
        {
            var version = new SdkVersion(major, minor, sdkMinor, patch, preview);

            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.SdkMinor.Should().Be(sdkMinor);
            version.Patch.Should().Be(patch);
            version.Preview.Should().Be(preview);
        }

        public static IEnumerable<object[]> GetDataForTestConstructorInvalid()
        {
            yield return new object[]
            {
                -2, 2, 3, 0, null
            };

            yield return new object[]
            {
                -1, 2, 3, 0, null
            };

            yield return new object[]
            {
                3, -1, 1, 0,
                new BundleVersion.PreviewVersion(5, 27363)
            };

            yield return new object[]
            {
                3, 0, -1, 0,
                new BundleVersion.PreviewVersion(null, 27363)
            };

            yield return new object[]
            {
                3, 0, 1, -1,
                new BundleVersion.PreviewVersion(null, 27363)
            };

            yield return new object[]
            {
                2, 2, 3, 100, null
            };

            yield return new object[]
            {
                2, 2, 3, 123,
                new BundleVersion.PreviewVersion(5, 27363)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestConstructorInvalid))]
        internal void TestConstructorInvalid(int major, int minor, int sdkMinor, int patch, BundleVersion.PreviewVersion preview)
        {
            Action action = () => new SdkVersion(major, minor, sdkMinor, patch, preview);
            action.Should().Throw<InvalidDataException>();
        }

        public static IEnumerable<object[]> GetDataForTestFromInputAccept()
        {
            yield return new object[]
            {
                "2.2.300",
                new SdkVersion(2, 2, 3, 0)
            };

            yield return new object[]
            {
                "2.1.202",
                new SdkVersion(2, 1, 2, 2)
            };

            yield return new object[]
            {
                "2.1.234",
                new SdkVersion(2, 1, 2, 34)
            };

            yield return new object[]
            {
                "3.0.100-preview5-27626",
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 27626))
            };

            yield return new object[]
           {
                "3.0.100-preview5-0",
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 0))
           };

            yield return new object[]
            {
                "3.0.123-preview-27626",
                new SdkVersion(3, 0, 1, 23, new BundleVersion.PreviewVersion(null, 27626))
            };

            yield return new object[]
            {
                "3.0.222-preview5-027626",
                new SdkVersion(3, 0, 2, 22, new BundleVersion.PreviewVersion(5, 27626))
            };

            yield return new object[]
            {
                "12.345.67891-preview011-121314",
                new SdkVersion(12, 345, 678, 91, new BundleVersion.PreviewVersion(11, 121314))
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFromInputAccept))]
        internal void TestFromInputAccept(string input, SdkVersion expected)
        {
            SdkVersion.FromInput(input)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2.2")]
        [InlineData("2.2.")]
        [InlineData("2.2.2")]
        [InlineData("2.2.22")]
        [InlineData("a.0.100")]
        [InlineData("0.a.100")]
        [InlineData("0.0.a00")]
        [InlineData("0.0.abc")]
        [InlineData("3.0.100-preview5")]
        [InlineData("3.0.100 - preview5-32768")]
        [InlineData("3.0.100-preview 5-32768")]
        [InlineData("3.0.100-preview-5-32768")]
        [InlineData("3.0.100-preview5 - 32768")]
        [InlineData("3.0.100 Preview 5-32768")]
        [InlineData("3.0.100 Preview 5 - 32768")]
        [InlineData("3.0.100-previewa-32768")]
        [InlineData("3.0.100-preview-a-32768")]
        [InlineData("3.0.100-preview5-abcde")]
        internal void TestFromInputReject(string input)
        {
            Action action = () => SdkVersion.FromInput(input);
            action.Should().Throw<InvalidInputVersionStringException>(
                string.Format(Messages.InvalidInputVersionStringExceptionMessageFormat, input));
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 42),
                new SdkVersion(2, 2, 3, 42)
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 32768)),
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 32768))
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(null, 32768)),
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(null, 32768))
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
                new SdkVersion(1, 2, 3, 0),
                new SdkVersion(2, 2, 3, 0)
            };

            yield return new object[]
            {
                new SdkVersion(2, 1, 3, 0),
                new SdkVersion(2, 2, 3, 0)
            };

            yield return new object[]
            {
                new SdkVersion(2, 2, 2, 2),
                new SdkVersion(2, 2, 3, 0)
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(4, 32768)),
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 32768))
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 32758)),
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 32768))
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(null, 32758)),
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(null, 32768))
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 32768)),
                new SdkVersion(3, 0, 1, 0)
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(null, 32768)),
                new SdkVersion(3, 0, 1, 0)
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
                new SdkVersion(2, 2, 3, 45)
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(5, 32768)),
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 0, new BundleVersion.PreviewVersion(null, 32768)),
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequalityNull))]
        internal void TestInequalityNull(SdkVersion version)
        {
            version.Equals(null).Should().BeFalse();
            version.CompareTo(null).Should().BeGreaterThan(0);
        }

        public static IEnumerable<object[]> GetDataForTestToString()
        {
            yield return new object[]
            {
                new SdkVersion(2, 2, 3, 0),
                "2.2.300"
            };

            yield return new object[]
            {
                new SdkVersion(1, 23, 456, 78),
                "1.23.45678"
            };
            yield return new object[]
            {
                new SdkVersion(1, 23, 456, 7),
                "1.23.45607"
            };


            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 2, new BundleVersion.PreviewVersion(5, 131072)),
                "3.0.102-preview5-131072"
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 2, new BundleVersion.PreviewVersion(null, 131072)),
                "3.0.102-preview-131072"
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 2, new BundleVersion.PreviewVersion(5, 32768)),
                "3.0.102-preview5-032768"
            };

            yield return new object[]
            {
                new SdkVersion(3, 0, 1, 23, new BundleVersion.PreviewVersion(5, 1024)),
                "3.0.123-preview5-001024"
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestToString))]
        internal void TestToString(SdkVersion version, string expected)
        {
            version.ToString().Should().Be(expected);
        }
    }
}
