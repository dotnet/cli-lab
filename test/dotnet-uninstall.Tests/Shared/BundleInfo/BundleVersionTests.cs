using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class PreviewVersionTests
    {
        [Theory]
        [InlineData(5, 23758)]
        [InlineData(null, 23758)]
        [InlineData(0, 23758)]
        [InlineData(5, 0)]
        [InlineData(0, 0)]
        internal void TestConstructor(int? previewNumber, int buildNumber)
        {
            var preview = new BundleVersion.PreviewVersion(previewNumber, buildNumber);

            preview.PreviewNumber.Should().Be(previewNumber);
            preview.BuildNumber.Should().Be(buildNumber);
        }

        [Theory]
        [InlineData(-1, 23758)]
        [InlineData(5, -23758)]
        [InlineData(null, -1)]
        [InlineData(-5, -23758)]
        internal void TestConstructorInvalid(int? previewNumber, int buildNumber)
        {
            Action action = () => new BundleVersion.PreviewVersion(previewNumber, buildNumber);
            action.Should().Throw<InvalidDataException>();
        }

        [Theory]
        [InlineData(5, 23758)]
        [InlineData(null, 23758)]
        internal void TestEquality(int? previewNumber, int buildNumber)
        {
            var preview1 = new BundleVersion.PreviewVersion(previewNumber, buildNumber);
            var preview2 = new BundleVersion.PreviewVersion(previewNumber, buildNumber);

            preview1.Equals((object)preview2).Should().BeTrue();
            preview1.CompareTo((object)preview2).Should().Be(0);
        }

        public static IEnumerable<object[]> GetDataForTestInequality()
        {
            yield return new object[]
            {
                new BundleVersion.PreviewVersion(5, 23758),
                new BundleVersion.PreviewVersion(6, 23758)
            };

            yield return new object[]
            {
                new BundleVersion.PreviewVersion(5, 23758),
                new BundleVersion.PreviewVersion(5, 23768)
            };

            yield return new object[]
            {
                new BundleVersion.PreviewVersion(null, 23758),
                new BundleVersion.PreviewVersion(5, 23758)
            };

            yield return new object[]
            {
                new BundleVersion.PreviewVersion(null, 23758),
                new BundleVersion.PreviewVersion(null, 23768)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequality))]
        internal void TestInequality(BundleVersion.PreviewVersion preview1, BundleVersion.PreviewVersion preview2)
        {
            preview1.Equals((object)preview2).Should().BeFalse();
            preview1.CompareTo((object)preview2).Should().BeLessThan(0);

            preview2.Equals((object)preview1).Should().BeFalse();
            preview2.CompareTo((object)preview1).Should().BeGreaterThan(0);
        }

        public static IEnumerable<object[]> GetDataForTestInequalityNull()
        {
            yield return new object[]
            {
                new BundleVersion.PreviewVersion(5, 32768)
            };

            yield return new object[]
            {
                new BundleVersion.PreviewVersion(null, 32768)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequalityNull))]
        internal void TestInequalityNull(BundleVersion.PreviewVersion preview)
        {
            preview.Equals(null).Should().BeFalse();
            preview.CompareTo(null).Should().BeLessThan(0);
        }

        [Theory]
        [InlineData(5, 23758, "D5", "-preview5-23758")]
        [InlineData(null, 23758, "D5", "-preview-23758")]
        [InlineData(5, 23758, "D6", "-preview5-023758")]
        [InlineData(null, 23758, "D6", "-preview-023758")]
        internal void TestToString(int? previewNumber, int buildNumber, string format, string expected)
        {
            new BundleVersion.PreviewVersion(previewNumber, buildNumber).ToString(format)
                .Should().Be(expected);
        }
    }
}
