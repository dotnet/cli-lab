using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
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
            return other == null ? 1 : _semVer.CompareTo(other._semVer);
        }
    }
}
