using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal class RuntimeVersion : BundleVersion, IEquatable<RuntimeVersion>, IComparable<RuntimeVersion>
    {
        public override BundleType Type { get; } = BundleType.Runtime;

        public RuntimeVersion(int major, int minor, int patch, PreviewVersion preview = null) : base(major, minor, patch, preview) { }

        public static RuntimeVersion FromInput(string input)
        {
            ParseFromInput(Regexes.RuntimeVersionInputRegex, input, out var major, out var minor, out var patch, out var preview, out var _);
            return new RuntimeVersion(major, minor, patch, preview);
        }

        public override bool Equals(BundleVersion other)
        {
            return Equals(other as RuntimeVersion);
        }

        public override int CompareTo(BundleVersion other)
        {
            if (other is RuntimeVersion)
            {
                return CompareTo(other as RuntimeVersion);
            }
            else
            {
                return 1;
            }
        }

        public bool Equals(RuntimeVersion other)
        {
            return other != null &&
                   Major == other.Major &&
                   Minor == other.Minor &&
                   Patch == other.Patch &&
                   (Preview == null ? other.Preview == null : Preview.Equals(other.Preview));
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

            if (Preview == other.Preview)
            {
                return 0;
            }

            return Preview == null ? 1 : Preview.CompareTo(other.Preview);
        }

        public override string ToString()
        {
            var previewString = Preview == null ? string.Empty : Preview.ToString("D5");
            return $"{Major}.{Minor}.{Patch}{previewString}";
        }
    }
}
