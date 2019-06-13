using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal class SdkVersion : BundleVersion, IEquatable<SdkVersion>, IComparable<SdkVersion>
    {
        public int SdkMinor { get; }
        public override BundleType Type { get; } = BundleType.Sdk;

        public SdkVersion(int major, int minor, int sdkMinor, int patch, PreviewVersion preview) : base(major, minor, patch, preview)
        {
            SdkMinor = sdkMinor;
        }

        public static SdkVersion FromInput(string input)
        {
            ParseFromInput(Regexes.SdkVersionInputRegex, input, out var major, out var minor, out var patch, out var preview, out var match);

            var sdkMinorString = match.Groups[Regexes.SdkMinorGroupName].Value;
            var sdkMinor = int.Parse(sdkMinorString);

            return new SdkVersion(major, minor, sdkMinor, patch, preview);
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
            var previewString = Preview == null ? string.Empty : Preview.ToString("D6");
            return $"{Major}.{Minor}.{SdkMinor}{Patch.ToString("D2")}{previewString}";
        }
    }
}
