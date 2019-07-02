using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Tests.TestUtils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    public class MajorMinorVersionTests
    {
        [Theory]
        [InlineData("1.0", 1, 0)]
        [InlineData("1.1", 1, 1)]
        [InlineData("2.0", 2, 0)]
        [InlineData("2.1", 2, 1)]
        [InlineData("2.2", 2, 2)]
        [InlineData("3.0", 3, 0)]
        [InlineData("12.345", 12, 345)]
        [InlineData("0012.00345", 12, 345)]
        internal void TestFromInput(string input, int major, int minor)
        {
            var majorMinor = MajorMinorVersion.FromInput(input);
            TestProperties(majorMinor, major, minor);

            MajorMinorVersion.TryFromInput(input, out majorMinor)
                .Should().BeTrue();
            TestProperties(majorMinor, major, minor);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2")]
        [InlineData("2.")]
        [InlineData("2.2.")]
        [InlineData("2.2.2")]
        [InlineData("2.2.202")]
        [InlineData("a.0")]
        [InlineData("0.a")]
        [InlineData("2.2-preview")]
        [InlineData("2.2-preview-011768")]
        [InlineData("2.2-preview-011768-15")]
        [InlineData("3.0.0-preview5-27626-15")]
        [InlineData("3.0.100-preview5-011568")]
        [InlineData("Hello2.2World")]
        [InlineData("Hello 2.2 World")]
        [InlineData("2. 2")]
        [InlineData("2 .2")]
        [InlineData("2 . 2")]
        internal void TestFromInputReject(string input)
        {
            Action action = () => MajorMinorVersion.FromInput(input);
            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));

            MajorMinorVersion.TryFromInput(input, out var majorMinor)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 0)]
        [InlineData(12, 345)]
        internal void TestConstructor(int major, int minor)
        {
            var majorMinor = new MajorMinorVersion(major, minor);

            TestProperties(majorMinor, major, minor);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(1, -1)]
        [InlineData(-12, -345)]
        internal void TestConstructorIntsArgumentOutOfRangeException(int major, int minor)
        {
            Action action = () => new MajorMinorVersion(major, minor);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        private void TestProperties(BeforePatch majorMinor, int major, int minor)
        {
            majorMinor.Major.Should().Be(major);
            majorMinor.Minor.Should().Be(minor);
        }

        [Theory]
        [InlineData("1.0", "1.0")]
        [InlineData("1.1", "1.1")]
        [InlineData("2.0", "2.0")]
        [InlineData("2.1", "2.1")]
        [InlineData("2.2", "2.2")]
        [InlineData("3.0", "3.0")]
        [InlineData("12.345", "0012.00345")]
        internal void TestEquality(string input1, string input2)
        {
            var majorMinor1 = MajorMinorVersion.FromInput(input1);
            var majorMinor2 = MajorMinorVersion.FromInput(input2);

            EqualityComparisonTestUtils<MajorMinorVersion>.TestEquality(majorMinor1, majorMinor2);
        }

        [Theory]
        [InlineData("1.0", "2.0")]
        [InlineData("2.1", "2.2")]
        [InlineData("1.2", "2.1")]
        [InlineData("6.66", "23.33")]
        internal void TestInequality(string lower, string higher)
        {
            var lowerMajorMinor = MajorMinorVersion.FromInput(lower);
            var higherMajorMinor = MajorMinorVersion.FromInput(higher);

            EqualityComparisonTestUtils<MajorMinorVersion>.TestInequality(lowerMajorMinor, higherMajorMinor);
        }

        [Theory]
        [InlineData("1.1")]
        [InlineData("12.345")]
        internal void TestInequalityNull(string input)
        {
            var majorMinor = MajorMinorVersion.FromInput(input);

            EqualityComparisonTestUtils<MajorMinorVersion>.TestInequalityNull(majorMinor);
        }
    }
}
