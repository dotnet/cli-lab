using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal abstract class BundleVersion : IEquatable<BundleVersion>, IComparable<BundleVersion>, IComparable
    {
        public int Major { get; protected set; }
        public int Minor { get; protected set; }
        public int Patch { get; protected set; }
        public PreviewVersion Preview { get; protected set; }
        public abstract BundleType Type { get; }

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
    }

    internal class PreviewVersion : IEquatable<PreviewVersion>, IComparable<PreviewVersion>, IComparable
    {
        public int PreviewNumber { get; }
        public int BuildNumber { get; }

        public PreviewVersion(int previewNumber, int buildNumber)
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
            return PreviewNumber != other.PreviewNumber ?
                PreviewNumber - other.PreviewNumber :
                BuildNumber - other.BuildNumber;
        }

        public override string ToString()
        {
            return $"-preview{PreviewNumber}-{BuildNumber}";
        }
    }
}
