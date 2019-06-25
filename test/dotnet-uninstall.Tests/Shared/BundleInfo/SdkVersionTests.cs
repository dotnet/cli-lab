using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class SdkVersionTests : BundleVersionTests<SdkVersion>
    {
        [Theory]
        [InlineData("2.2.300", 2, 2, 3, 0, false)]
        [InlineData("0.2.300", 0, 2, 3, 0, false)]
        [InlineData("2.0.300", 2, 0, 3, 0, false)]
        [InlineData("2.2.202", 2, 2, 2, 2, false)]
        [InlineData("3.0.100-preview5-011568", 3, 0, 1, 0, true)]
        [InlineData("2.0.0-rc", 2, 0, 0, 0, true)]
        [InlineData("2.0.2-rc1-abcdef", 2, 0, 0, 2, true)]
        internal void TestConstructor(string input, int major, int minor, int sdkMinor, int patch, bool isPrerelease)
        {
            var version = new SdkVersion(input);

            TestBundleVersionConstructor(version, major, minor, patch, isPrerelease, BundleType.Sdk);
            version.SdkMinor.Should().Be(sdkMinor);
        }

        [Theory]
        [InlineData("2.2.300", "2.2.300")]
        [InlineData("3.0.100-preview5-011568", "3.0.100-preview5-011568")]
        internal void TestEquality(string input1, string input2)
        {
            var version1 = new SdkVersion(input1);
            var version2 = new SdkVersion(input2);

            TestBundleVersionEquality(version1, version2);
        }

        [Theory]
        [InlineData("1.2.300", "2.2.300")]
        [InlineData("2.1.300", "2.2.300")]
        [InlineData("2.2.200", "2.2.300")]
        [InlineData("2.2.302", "2.2.342")]
        [InlineData("3.0.100-preview-009812", "3.0.100-preview5-011568")]
        [InlineData("3.0.100-preview5-011568", "3.0.100-rc1-008673")]
        [InlineData("3.0.100-preview5-011568", "3.0.100")]
        [InlineData("3.0.100-rc1-008673", "3.0.100")]
        internal void TestInequality(string lower, string higher)
        {
            var lowerVersion = new SdkVersion(lower);
            var higherVersion = new SdkVersion(higher);

            TestBundleVersionInequality(lowerVersion, higherVersion);
        }

        [Theory]
        [InlineData("2.2.300")]
        [InlineData("3.0.100-preview5-001568")]
        internal void TestInequalityNull(string input)
        {
            var version = new SdkVersion(input);

            TestBundleVersionInequalityNull(version);
        }

        [Theory]
        [InlineData("1.0.0-preview2-003121")]
        [InlineData("1.0.4")]
        [InlineData("1.1.14")]
        [InlineData("1.0.0-preview2.1-003177")]
        [InlineData("2.0.0-preview1-005977")]
        [InlineData("2.0.0")]
        [InlineData("2.1.100")]
        [InlineData("2.1.105")]
        [InlineData("2.1.202")]
        [InlineData("2.1.300-preview1-008174")]
        [InlineData("2.1.300-rc1-008673")]
        [InlineData("2.1.302")]
        [InlineData("2.2.300")]
        [InlineData("3.0.100-preview-009812")]
        [InlineData("3.0.100-preview6-012264")]
        [InlineData("2.0.0-preview")]
        [InlineData("2.0.0-preview1")]
        [InlineData("2.0.0-preview1-008174-01")]
        [InlineData("2.1.300-preview")]
        [InlineData("2.1.300-preview1")]
        [InlineData("2.0.300-preview1-008174-01")]
        [InlineData("2.1.300-rc")]
        [InlineData("2.1.300-rc1")]
        [InlineData("2.1.300-rc1-002111-01")]
        [InlineData("2.1.300-rc1-final")]
        [InlineData("2.0.100-preview1-abcdef")]
        internal void TestFromInputAccept(string input)
        {
            SdkVersion action() => new SdkVersion(input);

            TestBundleVersionNotThrow(action);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("-2.2.300")]
        [InlineData("-1.2.300")]
        [InlineData("2.2.-300")]
        [InlineData("3.-1.100-preview5-011568")]
        [InlineData("3.1.-100-preview5-011568")]
        [InlineData("3.1.-1-1-preview5-011568")]
        [InlineData("1.0")]
        [InlineData("1.0.")]
        [InlineData("12.345")]
        [InlineData("0012.00345")]
        [InlineData("2.2.5.002111")]
        [InlineData("2.2.500.002111")]
        [InlineData("a.0.100")]
        [InlineData("0.a.302")]
        [InlineData("0.0.abc")]
        [InlineData("Hello2.2.300World")]
        [InlineData("Hello 2.2.300 World")]
        internal void TestFromInputReject(string input)
        {
            SdkVersion action() => new SdkVersion(input);

            TestBundleVersionThrow<InvalidInputVersionException>(action, string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
