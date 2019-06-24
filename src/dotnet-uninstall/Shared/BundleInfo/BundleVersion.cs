using System;
using System.Text.RegularExpressions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal abstract class BundleVersion : IEquatable<BundleVersion>
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Build { get; }
        public bool Preview { get; }
        public abstract BundleType Type { get; }

        private string _displayVersion;

        public BundleVersion(int major, int minor, int patch, int build, bool preview, string displayVersion)
        {
            if (major < 0 || minor < 0 || patch < 0 || build < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            Major = major;
            Minor = minor;
            Patch = patch;
            Build = build;
            Preview = preview;
            _displayVersion = displayVersion ?? throw new ArgumentNullException();
        }

        public static BundleVersion FromInput<TBundleVersion>(string input)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>
        {
            if (typeof(TBundleVersion).Equals(typeof(SdkVersion)))
            {
                return SdkVersion.FromInput(input);
            }

            if (typeof(TBundleVersion).Equals(typeof(RuntimeVersion)))
            {
                return RuntimeVersion.FromInput(input);
            }

            throw new ArgumentException();
        }

        protected static void FromInput(Regex regex, string input, out int major, out int minor, out int patch, out int build, out bool preview, out Match match)
        {
            match = regex.Match(input);

            if (!match.Success)
            {
                throw new InvalidInputVersionException(input);
            }

            var majorString = match.Groups[Regexes.MajorGroupName].Value;
            var minorString = match.Groups[Regexes.MinorGroupName].Value;
            var patchString = match.Groups[Regexes.PatchGroupName].Value;
            var buildString = match.Groups[Regexes.BuildGroupName].Value;

            major = int.Parse(majorString);
            minor = int.Parse(minorString);
            patch = patchString.Equals(string.Empty) ? default : int.Parse(patchString);
            build = buildString.Equals(string.Empty) ? default : int.Parse(buildString);
            preview = match.Groups[Regexes.PreviewGroupName].Success;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BundleVersion);
        }

        public override string ToString()
        {
            return _displayVersion;
        }

        public abstract bool Equals(BundleVersion other);

        public abstract override int GetHashCode();
    }
}
