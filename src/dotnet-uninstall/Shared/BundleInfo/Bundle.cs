using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal abstract class Bundle : IEquatable<Bundle>
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

        public static Bundle From(BundleVersion version, BundleArch arch, string uninstallCommand)
        {
            switch (version.Type)
            {
                case BundleType.Sdk:
                    return new Bundle<SdkVersion>(version as SdkVersion, arch, uninstallCommand);
                case BundleType.Runtime:
                    return new Bundle<RuntimeVersion>(version as RuntimeVersion, arch, uninstallCommand);
                default:
                    throw new InvalidEnumArgumentException();
            }
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
            return $"{Version.ToString()} ({Arch.ToString().ToLower()})";
        }
    }

    internal class Bundle<TBundleVersion> : Bundle, IComparable, IComparable<Bundle<TBundleVersion>>
        where TBundleVersion: BundleVersion, IComparable<TBundleVersion>
    {
        public new TBundleVersion Version => base.Version as TBundleVersion;

        public Bundle(TBundleVersion version, BundleArch arch, string uninstallCommand) : base(version, arch, uninstallCommand) { }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Bundle<TBundleVersion>);
        }

        public int CompareTo(Bundle<TBundleVersion> other)
        {
            if (other == null)
            {
                return 1;
            }

            return Version.Equals(other.Version) ?
                Arch - other.Arch :
                Version.CompareTo(other.Version);
        }

        public static IEnumerable<Bundle<TBundleVersion>> FilterWithSameBundleType(IEnumerable<Bundle> bundles)
        {
            return bundles
                .Where(bundle => bundle.Version is TBundleVersion)
                .Select(bundle => bundle as Bundle<TBundleVersion>);
        }
    }
}
