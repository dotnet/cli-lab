using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
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

        public BundleVersion(int major, int minor, int patch, PreviewVersion preview = null)
        {
            if (major < 0 || minor < 0 || patch < 0)
            {
                throw new InvalidDataException();
            }

            Major = major;
            Minor = minor;
            Patch = patch;
            Preview = preview;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BundleVersion);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as BundleVersion);
        }

        public abstract bool Equals(BundleVersion other);

        public abstract override int GetHashCode();

        public abstract int CompareTo(BundleVersion other);

        public abstract override string ToString();

        protected static void ParseFromInput(Regex regex, string input, out int major, out int minor, out int patch, out PreviewVersion preview, out Match match)
        {
            match = regex.Match(input);

            if (!match.Success)
            {
                throw new InvalidInputVersionStringException(input);
            }

            var majorString = match.Groups[Regexes.MajorGroupName].Value;
            var minorString = match.Groups[Regexes.MinorGroupName].Value;
            var patchString = match.Groups[Regexes.PatchGroupName].Value;

            major = int.Parse(majorString);
            minor = int.Parse(minorString);
            patch = int.Parse(patchString);

            if (match.Groups[Regexes.PreviewGroupName].Success)
            {
                var previewNumber = match.Groups[Regexes.PreviewNumberGroupName].Success ?
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
                if (previewNumber != null && previewNumber.Value < 0 || buildNumber < 0)
                {
                    throw new InvalidDataException();
                }

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
                if (other == null)
                {
                    return -1;
                }

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
