using System;

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
