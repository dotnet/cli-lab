using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Version
{
    public class RuntimeVersion : BundleVersion, IComparable, IComparable<RuntimeVersion>
    {
        public override BundleType Type => BundleType.Runtime;

        public RuntimeVersion(string value) : base(value) { }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as RuntimeVersion);
        }

        public int CompareTo(RuntimeVersion other)
        {
            if (other == null)
            {
                return 1;
            }

            return SemVer.CompareTo(other.SemVer);
        }
    }
}
