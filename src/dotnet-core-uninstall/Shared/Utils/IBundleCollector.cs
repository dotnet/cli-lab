// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;

namespace Microsoft.DotNet.Tools.Uninstall.MacOs
{
    internal interface IBundleCollector
    {
        public IEnumerable<Bundle> GetAllInstalledBundles();

        public IEnumerable<BundleTypePrintInfo> GetSupportedBundleTypes();
    }
}
