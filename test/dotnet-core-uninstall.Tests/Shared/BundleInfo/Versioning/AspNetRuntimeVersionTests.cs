using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    public class AspNetRuntimeVersionTests
    {
        [Theory]
        [InlineData("2.2.5", 2, 2, 5, false)]
        [InlineData("0.2.5", 0, 2, 5, false)]
        [InlineData("2.1.0-rc1-final", 2, 1, 0, true)]
        [InlineData("2.1.0-preview2-final", 2, 1, 0, true)]
        [InlineData("3.0.0-preview-18579-0056", 3, 0, 0, true)]
        [InlineData("3.0.0-preview6.19307.2", 3, 0, 0, true)]
        internal void TestConstructor(string input, int major, int minor, int patch, bool isPrerelease)
        {
            TestProperties(new AspNetRuntimeVersion(input), major, minor, patch, isPrerelease, input);
            TestProperties(BundleVersion.FromInput<AspNetRuntimeVersion>(input), major, minor, patch, isPrerelease, input);
        }

        private static void TestProperties(AspNetRuntimeVersion version, int major, int minor, int patch, bool isPrerelease, string toStringExpected)
        {
            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.IsPrerelease.Should().Be(isPrerelease);
            version.MajorMinor.Should().Be(new MajorMinorVersion(major, minor));

            version.Type.Should().Be(BundleType.AspNetRuntime);
            version.BeforePatch.Should().Be(new MajorMinorVersion(major, minor));

            version.ToString().Should().Be(toStringExpected);
            version.ToString(true).Should().Be(toStringExpected);
            version.ToString(false).Should().Be(toStringExpected);
        }

        [Theory]
        [InlineData("2.2.5", "2.2.5")]
        [InlineData("2.1.0-preview2-final", "2.1.0-preview2-final")]
        internal void TestEquality(string input1, string input2)
        {
            var version1 = new AspNetRuntimeVersion(input1);
            var version2 = new AspNetRuntimeVersion(input2);

            TestUtils.EqualityComparisonTestUtils<AspNetRuntimeVersion>.TestEquality(version1, version2);
        }

        [Theory]
        [InlineData("1.2.5", "2.2.5")]
        [InlineData("2.1.5", "2.2.5")]
        [InlineData("2.2.4", "2.2.5")]
        [InlineData("3.0.0-preview-99999-01", "3.0.0-preview5-27626-15")]
        [InlineData("3.0.0-preview5-27122-01", "3.0.0-preview5-27626-15")]
        [InlineData("3.0.0-preview5-27626-15", "3.0.0-rc1-final")]
        [InlineData("3.0.0-preview5-27626-15", "3.0.0")]
        [InlineData("3.0.0-preview-18579-0056", "3.0.0-preview6.19307.2")]
        [InlineData("3.0.0-preview5-19227-01", "3.0.0-preview6.19307.2")]
        [InlineData("3.0.0-rc1-final", "3.0.0")]
        internal void TestInequality(string lower, string higher)
        {
            var lowerVersion = new AspNetRuntimeVersion(lower);
            var higherVersion = new AspNetRuntimeVersion(higher);

            TestUtils.EqualityComparisonTestUtils<AspNetRuntimeVersion>.TestInequality(lowerVersion, higherVersion);
        }

        [Theory]
        [InlineData("2.2.5")]
        [InlineData("3.0.0-preview5-27626-15")]
        internal void TestInequalityNull(string input)
        {
            var version = new AspNetRuntimeVersion(input);

            TestUtils.EqualityComparisonTestUtils<AspNetRuntimeVersion>.TestInequalityNull(version);
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
        [InlineData("3.0.0-preview.27122.1")]
        [InlineData("3.0.0-preview5.27626.15")]
        internal void TestFromInputAccept(string input)
        {
            Action action = () => new AspNetRuntimeVersion(input);

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
            Action action = () => new AspNetRuntimeVersion(input);

            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
