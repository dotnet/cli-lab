using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
{
    public class MajorMinorVersion : IEquatable<MajorMinorVersion>, IComparable, IComparable<MajorMinorVersion>
    {
        private readonly Version _version;

        public int Major => _version.Major;
        public int Minor => _version.Major;

        public MajorMinorVersion(string value)
        {
            if (!Regexes.BundleMajorMinorRegex.IsMatch(value) || !Version.TryParse(value, out _version))
            {
                throw new InvalidInputVersionException(value);
            }
        }

        public MajorMinorVersion(int major, int minor)
        {
            _version = new Version(major, minor);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as MajorMinorVersion);
        }

        public int CompareTo(MajorMinorVersion other)
        {
            return other == null ? 1 : _version.CompareTo(other._version);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MajorMinorVersion);
        }

        public bool Equals(MajorMinorVersion other)
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
