using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
{
    public class RuntimeVersion : BundleVersion, IComparable, IComparable<RuntimeVersion>, IEquatable<RuntimeVersion>
    {
        public override BundleType Type => BundleType.Runtime;

        public RuntimeVersion(string value) : base(value) { }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as RuntimeVersion);
        }

        public int CompareTo(RuntimeVersion other)
        {
            return other == null ? 1 : _semVer.CompareTo(other._semVer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RuntimeVersion);
        }

        public bool Equals(RuntimeVersion other)
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
