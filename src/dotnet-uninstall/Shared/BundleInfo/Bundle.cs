using System;
using System.Collections.Generic;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal abstract class Bundle : IEquatable<Bundle>, IComparable<Bundle>, IComparable
    {
        public abstract BundleVersion Version { get; }
        public abstract BundleArch Arch { get; }
        public abstract string UninstallCommand { get; }

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
            string archString = "";
            switch (Arch)
            {
                case BundleArch.X64: archString = "x64"; break;
                case BundleArch.X86: archString = "x86"; break;
                case BundleArch.Arm32: archString = "arm32"; break;
            }

            return $"{Version.ToString()} ({archString})";
        }
    }
}
