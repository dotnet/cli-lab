// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs
{
    internal class SupportedBundleTypeConfigs
    {
        public static IEnumerable<BundleTypePrintInfo> GetSupportedBundleTypes()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Windows.SupportedBundleTypeConfigs.SupportedBundleTypes;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return MacOs.SupportedBundleTypeConfigs.SupportedBundleTypes;
            }
            else
            {
                throw new OperatingSystemNotSupportedException();
            }
        }
    }
}
