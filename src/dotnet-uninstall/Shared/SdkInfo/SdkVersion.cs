using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo
{
    internal class SdkVersion : IEquatable<SdkVersion>, IComparable<SdkVersion>, IComparable
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int? Preview { get; }

        private string _versionString;

        public SdkVersion(string versionString)
        {
            var regex = Regexes.DotNetCoreVersionExtractionRegex;
            var match = regex.Match(versionString);

            if (!match.Success)
            {
                throw new InvalidVersionStringException(versionString);
            }

            var versionMajorString = match.Groups[Regexes.DotNetCoreVersionExtractionRegexMajorGroupName].Value;
            var versionMinorString = match.Groups[Regexes.DotNetCoreVersionExtractionRegexMinorGroupName].Value;
            var versionPatchString = match.Groups[Regexes.DotNetCoreVersionExtractionRegexPatchGroupName].Value;

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
