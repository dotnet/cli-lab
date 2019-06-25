using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    public class RuntimeVersionTests
    {
        [Theory]
        [InlineData("2.2.5", 2, 2, 5, false)]
        [InlineData("0.2.5", 0, 2, 5, false)]
        [InlineData("3.0.0-preview5-27626-15", 3, 0, 0, true)]
        [InlineData("3.0.1-rc1", 3, 0, 1, true)]
        [InlineData("3.0.1-rc1-final", 3, 0, 1, true)]
        internal void TestConstructor(string input, int major, int minor, int patch, bool isPrerelease)
        {
            var version = new RuntimeVersion(input);

            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.IsPrerelease.Should().Be(isPrerelease);
            version.MajorMinor.Should().Be(new MajorMinorVersion(major, minor));

            version.Type.Should().Be(BundleType.Runtime);
            version.BeforePatch.Should().Be(new MajorMinorVersion(major, minor));
        }

        [Theory]
        [InlineData("2.2.5", "2.2.5")]
        [InlineData("3.0.0-preview5-27626-15", "3.0.0-preview5-27626-15")]
        internal void TestEquality(string input1, string input2)
        {
            var version1 = new RuntimeVersion(input1);
            var version2 = new RuntimeVersion(input2);

            TestUtils.EqualityComparisonTestUtils<RuntimeVersion>.TestEquality(version1, version2);
        }

        [Theory]
        [InlineData("1.2.5", "2.2.5")]
        [InlineData("2.1.5", "2.2.5")]
        [InlineData("2.2.4", "2.2.5")]
        [InlineData("3.0.0-preview-99999-01", "3.0.0-preview5-27626-15")]
        [InlineData("3.0.0-preview5-27122-01", "3.0.0-preview5-27626-15")]
        [InlineData("3.0.0-preview5-27626-15", "3.0.0-rc1-final")]
        [InlineData("3.0.0-preview5-27626-15", "3.0.0")]
        [InlineData("3.0.0-rc1-final", "3.0.0")]
        internal void TestInequality(string lower, string higher)
        {
            var lowerVersion = new RuntimeVersion(lower);
            var higherVersion = new RuntimeVersion(higher);

            TestUtils.EqualityComparisonTestUtils<RuntimeVersion>.TestInequality(lowerVersion, higherVersion);
        }

        [Theory]
        [InlineData("2.2.5")]
        [InlineData("3.0.0-preview5-27626-15")]
        internal void TestInequalityNull(string input)
        {
            var version = new RuntimeVersion(input);

            TestUtils.EqualityComparisonTestUtils<RuntimeVersion>.TestInequalityNull(version);
        }

        [Theory]
        [InlineData("1.0.0")]
        [InlineData("1.0.16")]
        [InlineData("2.0.0-preview1-002111-00")]
        [InlineData("2.1.0-rc1")]
        [InlineData("2.2.5")]
        [InlineData("3.0.0-preview-27122-01")]
        [InlineData("3.0.0-preview5-27626-15")]
        [InlineData("2.0.0-preview")]
        [InlineData("2.0.0-preview1")]
        [InlineData("2.0.0-preview1-002111")]
        [InlineData("2.1.0-rc")]
        [InlineData("2.1.0-rc1-002111")]
        [InlineData("2.1.0-rc1-002111-01")]
        [InlineData("2.1.0-rc1-final")]
        [InlineData("2.0.0-preview1-abcdef-01")]
        [InlineData("2.0.0-preview1-002111-ab")]
        internal void TestFromInputAccept(string input)
        {
            Action action = () => new RuntimeVersion(input);

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("-2.2.5")]
        [InlineData("-1.2.5")]
        [InlineData("3.-1.0")]
        [InlineData("2.2.-5")]
        [InlineData("3.0.-1-preview5-27626-15")]
        [InlineData("1.0")]
        [InlineData("1.0.")]
        [InlineData("12.345")]
        [InlineData("0012.00345")]
        [InlineData("2.2.5.002111")]
        [InlineData("a.0.0")]
        [InlineData("0.a.0")]
        [InlineData("0.0.a")]
        [InlineData("Hello2.2.5World")]
        [InlineData("Hello 2.2.5 World")]
        internal void TestFromInputReject(string input)
        {
            Action action = () => new RuntimeVersion(input);

            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
