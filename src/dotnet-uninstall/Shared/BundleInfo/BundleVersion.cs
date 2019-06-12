using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal class BundleVersion : IEquatable<BundleVersion>, IComparable<BundleVersion>, IComparable
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int? Preview { get; }

        private string _versionString;

        public BundleVersion(string versionString)
        {
            var regex = Regexes.DotNetCoreBundleVersionRegex;
            var match = regex.Match(versionString);

            if (!match.Success)
            {
                throw new InvalidVersionStringException(versionString);
            }

            var versionMajorString = match.Groups[Regexes.VersionMajorGroupName].Value;
            var versionMinorString = match.Groups[Regexes.VersionMinorGroupName].Value;
            var versionPatchString = match.Groups[Regexes.VersionRegexPatchGroupName].Value;

            var versionPreviewString = match.Groups["preview"].Success ? match.Groups["preview"].Value : null;

            Major = int.Parse(versionMajorString);
            Minor = int.Parse(versionMinorString);
            Patch = int.Parse(versionPatchString);
            Preview = versionPreviewString != null ? int.Parse(versionPreviewString) as int? : null;

            _versionString = versionString;
        }

        public override string ToString()
        {
            return _versionString;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BundleVersion);
        }

        public bool Equals(BundleVersion other)
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

        public int CompareTo(BundleVersion other)
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
            return CompareTo(obj as BundleVersion);
        }
    }
}
