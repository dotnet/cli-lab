using System;
using System.Collections.Generic;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
{
    public abstract class BeforePatch : IEquatable<BeforePatch>
    {
        protected internal readonly Version _version;

        public int Major => _version.Major;
        public int Minor => _version.Minor;

        public BeforePatch(int major, int minor)
        {
            if (major < 0 || minor < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _version = new Version(major, minor);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BeforePatch);
        }

        public bool Equals(BeforePatch other)
        {
            return other != null &&
                   EqualityComparer<Version>.Default.Equals(_version, other._version);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_version);
        }
    }
}
