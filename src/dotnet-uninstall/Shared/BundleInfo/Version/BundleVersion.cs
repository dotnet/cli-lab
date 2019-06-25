using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Version
{
    public abstract class BundleVersion : IEquatable<BundleVersion>
    {
        public SemanticVersion SemVer { get; }
        public abstract BundleType Type { get; }

        public virtual int Major => SemVer.Major;
        public virtual int Minor => SemVer.Minor;
        public virtual int Patch => SemVer.Patch;
        public virtual bool IsPrerelease => SemVer.IsPrerelease;

        public BundleVersion(string value)
        {
            if (SemanticVersion.TryParse(value, out var version))
            {
                SemVer = version;
            }
            else
            {
                throw new InvalidInputVersionException(value);
            }
        }

        public override string ToString()
        {
            return SemVer.ToString();
        }

        public static BundleVersion FromInput<TBundleVersion>(string value)
            where TBundleVersion :BundleVersion
        {
            if (typeof(TBundleVersion).Equals(typeof(SdkVersion)))
            {
                return new SdkVersion(value);
            }

            if (typeof(TBundleVersion).Equals(typeof(RuntimeVersion)))
            {
                return new RuntimeVersion(value);
            }

            throw new ArgumentException();
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
