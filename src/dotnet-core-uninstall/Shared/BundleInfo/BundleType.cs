// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    [Flags]
    internal enum BundleType
    {
        Sdk = 0x1,
        Runtime = 0x2,
        AspNetRuntime = 0x4,
        HostingBundle = 0x8
    }
}
