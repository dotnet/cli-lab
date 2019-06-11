using System;
using System.Text.RegularExpressions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo
{
    internal class SdkVersion : IEquatable<SdkVersion>, IComparable<SdkVersion>, IComparable
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public int? Preview { get; set; }

        public SdkVersion(int major, int minor, int patch, int? preview = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Preview = preview;
        }

        public static SdkVersion From(string versionString)
        {
            var regex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)((\s\-\s|\-)preview(?<preview>\d+))?$");
            var match = regex.Match(versionString);

            if (!match.Success)
            {
                throw new InvalidVersionStringException(versionString);
            }

            var versionMajorString = match.Groups["major"].Value;
            var versionMinorString = match.Groups["minor"].Value;
            var versionPatchString = match.Groups["patch"].Value;

            var versionPreviewString = match.Groups["preview"].Success ? match.Groups["preview"].Value : null;

            var versionMajor = int.Parse(versionMajorString);
            var versionMinor = int.Parse(versionMinorString);
            var versionPatch = int.Parse(versionPatchString);
            var versionPreview = versionPreviewString != null ? int.Parse(versionPreviewString) as int? : null;

            return new SdkVersion(versionMajor, versionMinor, versionPatch, versionPreview);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}{3}", Major, Minor, Patch, Preview == null ? "" : "-preview" + Preview.Value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SdkVersion);
        }

        public bool Equals(SdkVersion other)
        {
            return other != null &&
                   Major == other.Major &&
                   Minor == other.Minor &&
                   Patch == other.Patch &&
                   Preview == other.Preview;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, Preview);
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

            if (Patch != other.Patch)
            {
                return Patch - other.Patch;
            }

            if (Preview == other.Preview)
            {
                return 0;
            }

            if (Preview == null)
            {
                return 1;
            }

            if (other.Preview == null)
            {
                return -1;
            }

            return Preview.Value - other.Preview.Value;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as SdkVersion);
        }
    }
}
