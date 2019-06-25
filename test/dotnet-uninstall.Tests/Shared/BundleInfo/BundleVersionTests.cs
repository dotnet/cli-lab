using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class BundleVersionTests<TBundleVersion>
        where TBundleVersion : BundleVersion, IComparable, IComparable<TBundleVersion>
    {
        internal void TestBundleVersionConstructor(TBundleVersion version, int major, int minor, int patch, bool isPrerelease, BundleType type)
        {
            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.IsPrerelease.Should().Be(isPrerelease);
            version.Type.Should().Be(type);
        }

        internal void TestBundleVersionEquality(TBundleVersion version1, TBundleVersion version2)
        {
            version1.Equals((object)version2).Should().BeTrue();
            version1.CompareTo((object)version2).Should().Be(0);

            version2.Equals((object)version1).Should().BeTrue();
            version2.CompareTo((object)version1).Should().Be(0);
        }

        internal void TestBundleVersionInequality(TBundleVersion lower, TBundleVersion higher)
        {
            lower.Equals((object)higher).Should().BeFalse();
            lower.CompareTo((object)higher).Should().BeLessThan(0);

            higher.Equals((object)lower).Should().BeFalse();
            higher.CompareTo((object)lower).Should().BeGreaterThan(0);
        }

        internal void TestBundleVersionInequalityNull(TBundleVersion version)
        {
            version.Equals(null).Should().BeFalse();
            version.CompareTo(null).Should().BeGreaterThan(0);
        }

        internal void TestBundleVersionNotThrow(Func<TBundleVersion> action)
        {
            action.Should().NotThrow();
        }

        internal void TestBundleVersionThrow<TException>(Func<TBundleVersion> action, string message)
            where TException : Exception
        {
            action.Should().Throw<TException>(message);
        }
    }
}
