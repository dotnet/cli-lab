using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
{
    internal abstract class BundleVersion : IEquatable<BundleVersion>
    {
        public abstract BundleType Type { get; }
        public abstract BeforePatch BeforePatch { get; }

        protected SemanticVersion SemVer { get; private set; }

        public virtual int Major => SemVer.Major;
        public virtual int Minor => SemVer.Minor;
        public virtual int Patch => SemVer.Patch;
        public virtual bool IsPrerelease => SemVer.IsPrerelease;
        public virtual MajorMinorVersion MajorMinor => new MajorMinorVersion(Major, Minor);

        protected BundleVersion()
        {
            SemVer = null;
        }

        protected BundleVersion(string value)
        {
            if (SemanticVersion.TryParse(value, out var semVer))
            {
                SemVer = semVer;
            }
            else
            {
                throw new InvalidInputVersionException(value);
            }
        }

        public static TBundleVersion FromInput<TBundleVersion>(string value)
            where TBundleVersion : BundleVersion, new()
        {
            if (SemanticVersion.TryParse(value, out var semVer))
            {
                return new TBundleVersion
                {
                    SemVer = semVer
                };
            }

            throw new InvalidInputVersionException(value);
        }

        public static bool TryFromInput<TBundleVersion>(string value, out TBundleVersion version)
            where TBundleVersion : BundleVersion, new()
        {
            if (SemanticVersion.TryParse(value, out var semVer))
            {
                version = new TBundleVersion
                {
                    SemVer = semVer
                };
                return true;
            }

            version = null;
            return false;
        }

        public override string ToString()
        {
            return SemVer.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BundleVersion);
        }

        public bool Equals(BundleVersion other)
        {
            return other != null &&
                   EqualityComparer<SemanticVersion>.Default.Equals(SemVer, other.SemVer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SemVer);
        }
    }
}
