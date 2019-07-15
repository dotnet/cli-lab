using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    public class HostingBundleVersionTests
    {
        [Theory]
        [InlineData("2.2.5", null, 2, 2, 5, false, false)]
        [InlineData("0.2.5", null, 0, 2, 5, false, false)]
        [InlineData("2.1.0-rc1-final", null, 2, 1, 0, true, false)]
        [InlineData("2.1.0-preview2-final", "test footnote", 2, 1, 0, true, true)]
        [InlineData("3.0.0-preview-18579-0056", "test footnote", 3, 0, 0, true, true)]
        [InlineData("3.0.0-preview6.19307.2", "test footnote", 3, 0, 0, true, true)]
        internal void TestConstructor(string input, string footnote, int major, int minor, int patch, bool isPrerelease, bool hasFootnote)
        {
            TestProperties(new HostingBundleVersion(input, footnote), footnote, major, minor, patch, isPrerelease, hasFootnote);
            TestProperties(BundleVersion.FromInput<HostingBundleVersion>(input, footnote), footnote, major, minor, patch, isPrerelease, hasFootnote);
        }

        private static void TestProperties(HostingBundleVersion version, string footnote, int major, int minor, int patch, bool isPrerelease, bool hasFootnote)
        {
            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.IsPrerelease.Should().Be(isPrerelease);
            version.MajorMinor.Should().Be(new MajorMinorVersion(major, minor));
            version.Footnote.Should().Be(footnote);

            version.Type.Should().Be(BundleType.HostingBundle);
            version.BeforePatch.Should().Be(new MajorMinorVersion(major, minor));
            version.HasFootnote.Should().Be(hasFootnote);
        }

        [Theory]
        [InlineData("2.2.5", "2.2.5")]
        [InlineData("2.1.0-preview2-final", "2.1.0-preview2-final")]
        internal void TestEquality(string input1, string input2)
        {
            var version1 = new HostingBundleVersion(input1);
            var version2 = new HostingBundleVersion(input2);

            TestUtils.EqualityComparisonTestUtils<HostingBundleVersion>.TestEquality(version1, version2);
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
            var lowerVersion = new HostingBundleVersion(lower);
            var higherVersion = new HostingBundleVersion(higher);

            TestUtils.EqualityComparisonTestUtils<HostingBundleVersion>.TestInequality(lowerVersion, higherVersion);
        }

        [Theory]
        [InlineData("2.2.5")]
        [InlineData("3.0.0-preview5-27626-15")]
        internal void TestInequalityNull(string input)
        {
            var version = new HostingBundleVersion(input);

            TestUtils.EqualityComparisonTestUtils<HostingBundleVersion>.TestInequalityNull(version);
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
            Action action = () => new HostingBundleVersion(input);

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
            Action action = () => new HostingBundleVersion(input);

            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
