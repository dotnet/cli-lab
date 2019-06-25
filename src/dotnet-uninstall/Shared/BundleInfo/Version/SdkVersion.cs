using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Version
{
    public class SdkVersion : BundleVersion, IComparable, IComparable<SdkVersion>
    {
        public int SdkMinor => SemVer.Patch / 100;
        public override int Patch => SemVer.Patch % 100;

        public override BundleType Type => BundleType.Sdk;

        public SdkVersion(string value) : base(value) { }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as SdkVersion);
        }

        public int CompareTo(SdkVersion other)
        {
            if (other == null)
            {
                return 1;
            }

            return SemVer.CompareTo(other.SemVer);
        }
    }
}
