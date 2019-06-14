using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal class RuntimeVersion : BundleVersion, IEquatable<RuntimeVersion>, IComparable, IComparable<RuntimeVersion>
    {
        public override BundleType Type { get; } = BundleType.Runtime;

        public RuntimeVersion(int major, int minor, int patch, int build, bool preview, string displayVersion) :
            base(major, minor, patch, build, preview, displayVersion)
        { }

        public static RuntimeVersion FromInput(string input)
        {
            FromInput(Regexes.RuntimeVersionCachePathRegex, input, out var major, out var minor, out var patch, out var build, out var preview, out var match);
            return new RuntimeVersion(major, minor, patch, build, preview, input);
        }

        public override bool Equals(BundleVersion other)
        {
            return Equals(other as RuntimeVersion);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, Build);
        }

        public bool Equals(RuntimeVersion other)
        {
            return other != null &&
                Major == other.Major &&
                Minor == other.Minor &&
                Patch == other.Patch &&
                Build == other.Build;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as RuntimeVersion);
        }

        public int CompareTo(RuntimeVersion other)
        {
            if (other == null)
            {
                return 1;
            }

            if (Major != other.Major)
            {
                return Major - other.Major;
            }

            if (Minor != other.Minor)
            {
                return Minor - other.Minor;
            }

            if (Patch != other.Patch)
            {
                return Patch - other.Patch;
            }

            return Build - other.Build;
        }
    }
}
