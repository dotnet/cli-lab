using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
{
    public class SdkVersion : BundleVersion, IComparable, IComparable<SdkVersion>, IEquatable<SdkVersion>
    {
        public int SdkMinor => _semVer.Patch / 100;
        public override int Patch => _semVer.Patch % 100;
        public override BeforePatch BeforePatch => new MajorMinorSdkMinorVersion(Major, Minor, SdkMinor);

        public override BundleType Type => BundleType.Sdk;

        public SdkVersion(string value) : base(value) { }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as SdkVersion);
        }

        public int CompareTo(SdkVersion other)
        {
            return other == null ? 1 : _semVer.CompareTo(other._semVer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SdkVersion);
        }

        public bool Equals(SdkVersion other)
        {
            return other != null &&
                   base.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode());
        }
    }
}
