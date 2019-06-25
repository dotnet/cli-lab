using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
{
    public class MajorMinorSdkMinorVersion : MajorMinorVersion, IEquatable<MajorMinorSdkMinorVersion>, IComparable<MajorMinorSdkMinorVersion>
    {
        public int SdkMinor { get; }

        public MajorMinorSdkMinorVersion(int major, int minor, int sdkMinor) : base(major, minor)
        {
            SdkMinor = sdkMinor;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MajorMinorSdkMinorVersion);
        }

        public bool Equals(MajorMinorSdkMinorVersion other)
        {
            return other != null &&
                   base.Equals(other) &&
                   SdkMinor == other.SdkMinor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), SdkMinor);
        }

        public int CompareTo(MajorMinorSdkMinorVersion other)
        {
            if (other == null)
            {
                return 1;
            }

            return base.Equals(other) ?
                SdkMinor.CompareTo(other.SdkMinor) :
                base.CompareTo(other);
        }
    }
}
