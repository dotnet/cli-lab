using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal class RuntimeVersion : BundleVersion, IEquatable<RuntimeVersion>, IComparable<RuntimeVersion>
    {
        public override BundleType Type { get; } = BundleType.Runtime;

        public RuntimeVersion(int major, int minor, int patch, PreviewVersion preview = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Preview = preview;
        }

        public override bool Equals(BundleVersion other)
        {
            return Equals(other as RuntimeVersion);
        }

        public override int CompareTo(BundleVersion other)
        {
            return CompareTo(other as RuntimeVersion);
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

            return Preview == null ? 1 : Preview.CompareTo(other.Preview);
        }

        public override string ToString()
        {
            var previewString = Preview == null ? string.Empty : Preview.ToString();
            return $"{Major}.{Minor}.{Patch}{previewString}";
        }
    }
}
