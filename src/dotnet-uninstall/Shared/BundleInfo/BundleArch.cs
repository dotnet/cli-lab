using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    [Flags]
    internal enum BundleArch
    {
        X86 = 0x1,
        X64 = 0x2
    }
}
