using System;
using FluentAssertions;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.TestUtils
{
    public static class EqualityComparisonTestUtils<T>
        where T : IEquatable<T>, IComparable, IComparable<T>
    {
        internal static void TestEquality(T obj1, T obj2)
        {
            obj1.Equals((object)obj2).Should().BeTrue();
            obj1.CompareTo((object)obj2).Should().Be(0);

            obj2.Equals((object)obj1).Should().BeTrue();
            obj2.CompareTo((object)obj1).Should().Be(0);
        }

        internal static void TestInequality(T lower, T higher)
        {
            lower.Equals((object)higher).Should().BeFalse();
            lower.CompareTo((object)higher).Should().BeLessThan(0);

            higher.Equals((object)lower).Should().BeFalse();
            higher.CompareTo((object)lower).Should().BeGreaterThan(0);
        }

        internal static void TestInequalityNull(T obj)
        {
            obj.Equals(null).Should().BeFalse();
            obj.CompareTo(null).Should().BeGreaterThan(0);
        }
    }
}
