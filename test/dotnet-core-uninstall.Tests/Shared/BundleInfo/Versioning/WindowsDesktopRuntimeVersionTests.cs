using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    public class WindowsDesktopRuntimeVersionTests
    {
        [Theory]
        [InlineData("3.1.8", 3, 1, 8, false)]
        [InlineData("3.1.32", 3, 1, 32, false)]
        [InlineData("5.0.4", 5, 0, 4, false)]
        [InlineData("5.0.17", 5, 0, 17, false)]
        [InlineData("6.0.5", 6, 0, 5, false)]
        [InlineData("6.0.13", 6, 0, 13, false)]
        [InlineData("7.0.2", 7, 0, 2, false)]
        [InlineData("7.0.0-preview.1.22076.8", 7, 0, 0, true)]
        [InlineData("8.0.0-rc.2.23479.6", 8, 0, 0, true)]
        internal void TestConstructor(string input, int major, int minor, int patch, bool isPrerelease)
        {
            TestProperties(new WindowsDesktopRuntimeVersion(input), major, minor, patch, isPrerelease, input);
            TestProperties(BundleVersion.FromInput<WindowsDesktopRuntimeVersion>(input), major, minor, patch, isPrerelease, input);
        }

        private static void TestProperties(WindowsDesktopRuntimeVersion version, int major, int minor, int patch, bool isPrerelease, string toStringExpected)
        {
            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.IsPrerelease.Should().Be(isPrerelease);
            version.MajorMinor.Should().Be(new MajorMinorVersion(major, minor));

            version.Type.Should().Be(BundleType.WindowsDesktopRuntime);
            version.BeforePatch.Should().Be(new MajorMinorVersion(major, minor));

            version.ToString().Should().Be(toStringExpected);
            version.ToStringWithAsterisk().Should().Be(toStringExpected);
        }

        [Theory]
        [InlineData("3.1.8", "3.1.8")]
        [InlineData("7.0.0-preview.1.22076.8", "7.0.0-preview.1.22076.8")]
        internal void TestEquality(string input1, string input2)
        {
            var version1 = new WindowsDesktopRuntimeVersion(input1);
            var version2 = new WindowsDesktopRuntimeVersion(input2);

            TestUtils.EqualityComparisonTestUtils<WindowsDesktopRuntimeVersion>.TestEquality(version1, version2);
        }

        [Theory]
        [InlineData("3.1.8", "3.1.32")]
        [InlineData("5.0.4", "5.0.17")]
        [InlineData("6.0.5", "6.0.13")]
        [InlineData("6.0.13", "7.0.2")]
        [InlineData("7.0.0-preview.1.22076.8", "7.0.0-rc.1.23419.4")]
        [InlineData("7.0.0-rc.1.23419.4", "7.0.0")]
        [InlineData("3.1.32", "5.0.4")]
        internal void TestInequality(string lower, string higher)
        {
            var lowerVersion = new WindowsDesktopRuntimeVersion(lower);
            var higherVersion = new WindowsDesktopRuntimeVersion(higher);

            TestUtils.EqualityComparisonTestUtils<WindowsDesktopRuntimeVersion>.TestInequality(lowerVersion, higherVersion);
        }

        [Theory]
        [InlineData("3.1.8")]
        [InlineData("7.0.0-preview.1.22076.8")]
        internal void TestInequalityNull(string input)
        {
            var version = new WindowsDesktopRuntimeVersion(input);

            TestUtils.EqualityComparisonTestUtils<WindowsDesktopRuntimeVersion>.TestInequalityNull(version);
        }

        [Theory]
        [InlineData("3.1.8")]
        [InlineData("3.1.32")]
        [InlineData("5.0.4")]
        [InlineData("5.0.17")]
        [InlineData("6.0.5")]
        [InlineData("6.0.13")]
        [InlineData("7.0.2")]
        [InlineData("7.0.0-preview.1.22076.8")]
        [InlineData("8.0.0-rc.2.23479.6")]
        [InlineData("3.0.0-preview")]
        [InlineData("3.0.0-preview1")]
        [InlineData("3.0.0-preview1-002111")]
        [InlineData("3.1.0-rc")]
        [InlineData("3.1.0-rc1-002111")]
        [InlineData("3.1.0-rc1-002111-01")]
        [InlineData("3.1.0-rc1-final")]
        internal void TestFromInputAccept(string input)
        {
            Action action = () => new WindowsDesktopRuntimeVersion(input);

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("-3.1.8")]
        [InlineData("-1.2.5")]
        [InlineData("3.-1.0")]
        [InlineData("3.1.-8")]
        [InlineData("3.0.-1-preview.1.22076.8")]
        [InlineData("3.1")]
        [InlineData("3.1.")]
        [InlineData("12.345")]
        [InlineData("0012.00345")]
        [InlineData("3.1.8.002111")]
        [InlineData("a.0.0")]
        [InlineData("0.a.0")]
        [InlineData("0.0.a")]
        [InlineData("Hello3.1.8World")]
        [InlineData("Hello 3.1.8 World")]
        internal void TestFromInputReject(string input)
        {
            Action action = () => new WindowsDesktopRuntimeVersion(input);

            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
