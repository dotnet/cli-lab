using System;
using System.Collections.Generic;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal class Bundle : IEquatable<Bundle>, IComparable<Bundle>, IComparable
    {
        public BundleVersion Version { get; }
        public BundleArch Arch { get; }
        public string UninstallCommand { get; }

        public Bundle(BundleVersion version, BundleArch arch, string uninstallCommand)
        {
            if (version == null || uninstallCommand == null)
            {
                throw new ArgumentNullException();
            }

            Version = version;
            Arch = arch;
            UninstallCommand = uninstallCommand;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Bundle);
        }

        public int CompareTo(Bundle other)
        {
            return Version.Equals(other.Version) ?
                Arch.CompareTo(other.Arch) :
                Version.CompareTo(other.Version);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Bundle);
        }

        public bool Equals(Bundle other)
        {
            return other != null &&
                   EqualityComparer<BundleVersion>.Default.Equals(Version, other.Version) &&
                   Arch == other.Arch;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version, Arch);
        }

        public override string ToString()
        {
            return $"{Version.ToString()} ({Arch.ToString()})";
        }
    }
}
