using System;
using System.Text.RegularExpressions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal abstract class BundleVersion : IEquatable<BundleVersion>, IComparable<BundleVersion>, IComparable
    {
        public int Major { get; protected set; }
        public int Minor { get; protected set; }
        public int Patch { get; protected set; }
        public PreviewVersion Preview { get; protected set; }
        public abstract BundleType Type { get; }

        public BundleVersion(int major, int minor, int patch, PreviewVersion preview)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Preview = preview;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BundleVersion);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, Preview);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as BundleVersion);
        }

        public abstract bool Equals(BundleVersion other);

        public abstract int CompareTo(BundleVersion other);

        public abstract override string ToString();

        protected static void ParseFromInput(Regex regex, string input, out int major, out int minor, out int patch, out PreviewVersion preview, out Match match)
        {
            match = regex.Match(input);

            var majorString = match.Groups[Regexes.MajorGroupName].Value;
            var minorString = match.Groups[Regexes.MinorGroupName].Value;
            var patchString = match.Groups[Regexes.PatchGroupName].Value;

            major = int.Parse(majorString);
            minor = int.Parse(minorString);
            patch = int.Parse(patchString);

            if (match.Groups[Regexes.PreviewNumberGroupName].Success)
            {
                var previewNumber = match.Groups[Regexes.PreviewGroupName].Success ?
                    int.Parse(match.Groups[Regexes.PreviewNumberGroupName].Value) as int? :
                    null;

                var buildNumberString = match.Groups[Regexes.BuildNumberGroupName].Value;
                var buildNumber = int.Parse(buildNumberString);

                preview = new PreviewVersion(previewNumber, buildNumber);
            }
            else
            {
                preview = null;
            }
        }

        internal class PreviewVersion : IEquatable<PreviewVersion>, IComparable<PreviewVersion>, IComparable
        {
            public int? PreviewNumber { get; }
            public int BuildNumber { get; }

            public PreviewVersion(int? previewNumber, int buildNumber)
            {
                PreviewNumber = previewNumber;
                BuildNumber = buildNumber;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as PreviewVersion);
            }

            public bool Equals(PreviewVersion other)
            {
                return other != null &&
                       PreviewNumber == other.PreviewNumber &&
                       BuildNumber == other.BuildNumber;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(PreviewNumber, BuildNumber);
            }

            public int CompareTo(object obj)
            {
                return CompareTo(obj as PreviewVersion);
            }

            public int CompareTo(PreviewVersion other)
            {
                if (PreviewNumber == other.PreviewNumber)
                {
                    return BuildNumber - other.BuildNumber;
                }

                if (PreviewNumber == null || other.PreviewNumber == null)
                {
                    return PreviewNumber == null ? -1 : 1;
                }
                else
                {
                    return PreviewNumber.Value - other.PreviewNumber.Value;
                }
            }

            public string ToString(string buildNumberFormat)
            {
                var previewNumberString = PreviewNumber == null ?
                    string.Empty :
                    PreviewNumber.Value.ToString();

                return $"-preview{previewNumberString}-{BuildNumber.ToString(buildNumberFormat)}";
            }
        }
    }
}
