// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllLowerPatchesOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var highestVersions = new Dictionary<BeforePatch, TBundleVersion>();

            foreach (var version in bundles
                .Select(bundle => bundle.Version))
            {
                if (highestVersions.TryGetValue(version.BeforePatch, out var highest))
                {
                    if (version.CompareTo(highest) > 0)
                    {
                        highestVersions[version.BeforePatch] = version;
                    }
                }
                else
                {
                    highestVersions.Add(version.BeforePatch, version);
                }
            }

            return bundles
                .Where(bundle =>
                {
                    var version = bundle.Version;
                    highestVersions.TryGetValue(version.BeforePatch, out var highest);
                    return version.CompareTo(highest) < 0;
                });
        }
    }
}
