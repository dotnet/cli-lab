using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    [Flags]
    internal enum BundleType
    {
        Sdk = 0x1,
        Runtime = 0x2
    }
}
