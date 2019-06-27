﻿using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
{
    internal class SdkVersion : BundleVersion, IComparable, IComparable<SdkVersion>, IEquatable<SdkVersion>
    {
        public int SdkMinor => SemVer.Patch / 100;
        public override int Patch => SemVer.Patch % 100;
        public override BeforePatch BeforePatch => new MajorMinorSdkMinorVersion(Major, Minor, SdkMinor);

        public override BundleType Type => BundleType.Sdk;

        public SdkVersion() { }

        public SdkVersion(string value) : base(value) { }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as SdkVersion);
        }

        public int CompareTo(SdkVersion other)
        {
            return other == null ? 1 : SemVer.CompareTo(other.SemVer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SdkVersion);
        }

        public bool Equals(SdkVersion other)
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
