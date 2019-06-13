using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal class SdkVersion : BundleVersion, IEquatable<SdkVersion>, IComparable<SdkVersion>
    {
        public int SdkMinor { get; }
        public override BundleType Type { get; } = BundleType.Sdk;

        public SdkVersion(int major, int minor, int sdkMinor, int patch, PreviewVersion preview)
        {
            Major = major;
            Minor = minor;
            SdkMinor = sdkMinor;
            Patch = patch;
            Preview = preview;
        }

        public override bool Equals(BundleVersion other)
        {
            return Equals(other as SdkVersion);
        }

        public override int CompareTo(BundleVersion other)
        {
            return CompareTo(other as SdkVersion);
        }

        public bool Equals(SdkVersion other)
        {
            return other != null &&
                   Major == other.Major &&
                   Minor == other.Minor &&
                   SdkMinor == other.SdkMinor &&
                   Patch == other.Patch &&
                   (Preview == null ? other.Preview == null : Preview.Equals(other.Preview));
        }

        public int CompareTo(SdkVersion other)
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

            if (SdkMinor != other.SdkMinor)
            {
                return SdkMinor - other.SdkMinor;
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
            return $"{Major}.{Minor}.{SdkMinor}{Patch}{previewString}";
        }
    }
}
