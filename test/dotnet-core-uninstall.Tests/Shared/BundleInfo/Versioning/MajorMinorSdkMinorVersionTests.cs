using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Tests.TestUtils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    public class MajorMinorSdkMinorVersionTests
    {
        [Theory]
        [InlineData(1, 0, 0)]
        [InlineData(1, 1, 0)]
        [InlineData(2, 0, 1)]
        [InlineData(2, 1, 7)]
        [InlineData(2, 2, 3)]
        [InlineData(3, 0, 1)]
        [InlineData(12, 345, 7890)]
        internal void TestConstructorInts(int major, int minor, int sdkMinor)
        {
            var majorMinor = new MajorMinorSdkMinorVersion(major, minor, sdkMinor);

            majorMinor.Major.Should().Be(major);
            majorMinor.Minor.Should().Be(minor);
            majorMinor.SdkMinor.Should().Be(sdkMinor);
        }

        [Theory]
        [InlineData(-1, 0, 0)]
        [InlineData(1, -1, 1)]
        [InlineData(2, 0, -1)]
        [InlineData(-12, -345, 7890)]
        [InlineData(-12, 345, -7890)]
        [InlineData(12, -345, -7890)]
        [InlineData(-12, -345, -7890)]
        internal void TestConstructorIntsArgumentOutOfRangeException(int major, int minor, int sdkMinor)
        {
            Action action = () => new MajorMinorSdkMinorVersion(major, minor, sdkMinor);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(1, 0, 1),
                new MajorMinorSdkMinorVersion(1, 0, 1)
            };

            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(12, 345, 6789),
                new MajorMinorSdkMinorVersion(12, 345, 6789)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestEquality))]
        internal void TestEquality(MajorMinorSdkMinorVersion majorMinor1, MajorMinorSdkMinorVersion majorMinor2)
        {
            EqualityComparisonTestUtils<MajorMinorSdkMinorVersion>.TestEquality(majorMinor1, majorMinor2);
        }

        public static IEnumerable<object[]> GetDataForTestInequality()
        {
            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(1, 0, 1),
                new MajorMinorSdkMinorVersion(2, 0, 1)
            };

            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(2, 1, 7),
                new MajorMinorSdkMinorVersion(2, 2, 3)
            };

            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(12, 345, 678),
                new MajorMinorSdkMinorVersion(12, 345, 6789)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequality))]
        internal void TestInequality(MajorMinorSdkMinorVersion lower, MajorMinorSdkMinorVersion higher)
        {
            EqualityComparisonTestUtils<MajorMinorSdkMinorVersion>.TestInequality(lower, higher);
        }

        public static IEnumerable<object[]> GetDataForTestInequalityNull()
        {
            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(1, 0, 1)
            };

            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(12, 345, 6789)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequalityNull))]
        internal void TestInequalityNull(MajorMinorSdkMinorVersion majorMinor)
        {
            EqualityComparisonTestUtils<MajorMinorSdkMinorVersion>.TestInequalityNull(majorMinor);
        }
    }
}
