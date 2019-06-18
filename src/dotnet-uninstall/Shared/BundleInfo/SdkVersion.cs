using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal class SdkVersion : BundleVersion, IEquatable<SdkVersion>, IComparable, IComparable<SdkVersion>
    {
        public int SdkMinor { get; }
        public override BundleType Type => BundleType.Sdk;

        public SdkVersion(int major, int minor, int sdkMinor, int patch, int build, bool preview, string displayVersion) :
            base(major, minor, patch, build, preview, displayVersion)
        {
            if (sdkMinor < 0 || patch >= 100)
            {
                throw new ArgumentOutOfRangeException();
            }

            SdkMinor = sdkMinor;
        }

        public static SdkVersion FromInput(string input)
        {
            FromInput(Regexes.SdkVersionInputRegex, input, out var major, out var minor, out var patch, out var build, out var preview, out var match);

            var sdkMinorString = match.Groups[Regexes.SdkMinorGroupName].Value;
            var sdkMinor = sdkMinorString.Equals(string.Empty) ? default : int.Parse(sdkMinorString);

            return new SdkVersion(major, minor, sdkMinor, patch, build, preview, input);
        }

        public override bool Equals(BundleVersion other)
        {
            return Equals(other as SdkVersion);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, SdkMinor, Patch, Build);
        }

        public bool Equals(SdkVersion other)
        {
            return other != null &&
                Major == other.Major &&
                Minor == other.Minor &&
                SdkMinor == other.SdkMinor &&
                Patch == other.Patch &&
                Build == other.Build;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as SdkVersion);
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

            return Build - other.Build;
        }
    }
}
