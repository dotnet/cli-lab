// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning
{
    internal class WindowsDesktopRuntimeVersion : BundleVersion, IComparable, IComparable<WindowsDesktopRuntimeVersion>, IEquatable<WindowsDesktopRuntimeVersion>
    {
        public override BundleType Type => BundleType.WindowsDesktopRuntime;
        public override BeforePatch BeforePatch => new MajorMinorVersion(Major, Minor);

        public WindowsDesktopRuntimeVersion() { }

        public WindowsDesktopRuntimeVersion(string value) : base(value) { }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as WindowsDesktopRuntimeVersion);
        }

        public int CompareTo(WindowsDesktopRuntimeVersion other)
        {
            return other == null ? 1 : SemVer.CompareTo(other.SemVer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WindowsDesktopRuntimeVersion);
        }

        public bool Equals(WindowsDesktopRuntimeVersion other)
        {
            return other != null &&
                   base.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode());
        }

        public override Bundle ToBundle(BundleArch arch, string uninstallCommand, string displayName)
        {
            return new Bundle<WindowsDesktopRuntimeVersion>(this, arch, uninstallCommand, displayName);
        }
    }
}
