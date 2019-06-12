using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class BundleVersionTests
    {
        [Theory]
        [InlineData("2.2.5", 2, 2, 5, null)]
        [InlineData("3.0.0 Preview 5", 3, 0, 0, 5)]
        [InlineData("2.2.300", 2, 2, 300, null)]
        [InlineData("3.0.100 - preview5", 3, 0, 100, 5)]
        [InlineData("3.0.100-preview5", 3, 0, 100, 5)]
        [InlineData("03.0.100 - preview5", 3, 0, 100, 5)]
        [InlineData("3.00.100 - preview5", 3, 0, 100, 5)]
        [InlineData("3.0.0100 - preview5", 3, 0, 100, 5)]
        [InlineData("3.0.100 - preview05", 3, 0, 100, 5)]
        [InlineData("03.0.0 Preview 5", 3, 0, 0, 5)]
        [InlineData("3.00.0 Preview 5", 3, 0, 0, 5)]
        [InlineData("3.0.00 Preview 5", 3, 0, 0, 5)]
        [InlineData("3.0.0 Preview 05", 3, 0, 0, 5)]
        [InlineData("3.0.100 Preview 5", 3, 0, 100, 5)]
        public void TestConstructor(string versionString, int major, int minor, int patch, int? preview)
        {
            var version = new BundleVersion(versionString);

            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.Preview.Should().Be(preview);

            version.ToString().Should().Be(versionString);
        }

        [Theory]
        [InlineData("3.0.0 - preview5")]
        [InlineData("3.0.10 - preview5")]
        [InlineData("3.0.0 - preview 5")]
        [InlineData("a.0.0")]
        [InlineData("0.a.0")]
        [InlineData("0.0.a")]
        [InlineData("2.2.300 - preview")]
        [InlineData("3.0.0 Preview")]
        [InlineData("2.2.300 - previewA")]
        [InlineData("3.0.0 Preview A")]
        [InlineData("2.2.300 - preview 5")]
        [InlineData("2.2.300 - Preview5")]
        [InlineData("3.0.0 Preview5")]
        [InlineData("3.0.0 preview 5")]
        public void TestConstructorException(string versionString)
        {
            Action action = () => new BundleVersion(versionString);
            action.Should().Throw<InvalidVersionStringException>(versionString);
        }

        [Theory]
        [InlineData("2.2.5", "2.2.5")]
        [InlineData("3.0.0 Preview 5", "03.0.00 Preview 005")]
        [InlineData("3.0.100 Preview 5", "3.0.100 - preview5")]
        public void TestEquality(string versionString1, string versionString2)
        {
            var bundleVersion1 = new BundleVersion(versionString1);
            var bundleVersion2 = new BundleVersion(versionString2);

            bundleVersion1.Equals((object)bundleVersion2).Should().BeTrue();
            bundleVersion1.CompareTo((object)bundleVersion2).Should().Be(0);
        }

        [Fact]
        public void TestEqualsNull()
        {
            new BundleVersion("2.2.5").Equals((object)null)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("1.8.5", "2.2.5")]
        [InlineData("2.1.8", "2.2.5")]
        [InlineData("2.2.3", "2.2.5")]
        [InlineData("1.2.5 Preview 3", "2.2.5 Preview 5")]
        [InlineData("2.1.5 Preview 8", "2.2.5 Preview 5")]
        [InlineData("2.2.4 Preview 0", "2.2.5 Preview 5")]
        [InlineData("3.0.100 - preview5", "3.0.100")]
        [InlineData("3.0.100 - preview5", "3.0.100 - preview6")]
        public void TestInequality(string versionString1, string versionString2)
        {
            var bundleVersion1 = new BundleVersion(versionString1);
            var bundleVersion2 = new BundleVersion(versionString2);

            bundleVersion1.Equals((object)bundleVersion2).Should().BeFalse();
            bundleVersion1.CompareTo((object)bundleVersion2).Should().BeLessThan(0);
            bundleVersion2.CompareTo((object)bundleVersion1).Should().BeGreaterThan(0);
        }

        [Fact]
        public void TestCompareToNull()
        {
            new BundleVersion("2.2.5").CompareTo((object)null)
                .Should().Be(1);
        }
    }
}
