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

        protected readonly SemanticVersion _semVer;

        public virtual int Major => _semVer.Major;
        public virtual int Minor => _semVer.Minor;
        public virtual int Patch => _semVer.Patch;
        public virtual bool IsPrerelease => _semVer.IsPrerelease;
        public virtual MajorMinorVersion MajorMinor => new MajorMinorVersion(Major, Minor);

        public BundleVersion(string value)
        {
            if (SemanticVersion.TryParse(value, out var version))
            {
                _semVer = version;
            }
            else
            {
                throw new InvalidInputVersionException(value);
            }
        }

        public override string ToString()
        {
            return _semVer.ToString();
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
                   EqualityComparer<SemanticVersion>.Default.Equals(_semVer, other._semVer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_semVer);
        }
    }
}
