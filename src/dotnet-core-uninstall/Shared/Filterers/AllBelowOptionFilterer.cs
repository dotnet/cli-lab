// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllBelowOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(string argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            if (MajorMinorVersion.TryFromInput(argValue, out var majorMinor))
            {
                return bundles
                    .Where(bundle => bundle.Version.MajorMinor.CompareTo(majorMinor) < 0);
            }
            else
            {
                var version = BundleVersion.FromInput<TBundleVersion>(argValue) as TBundleVersion;

                return bundles
                    .Where(bundle => bundle.Version.CompareTo(version) < 0);
            }
        }
    }
}
