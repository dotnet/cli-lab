// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllButLatestOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var latestSdk = bundles
                .Select(bundle => bundle.Version)
                .Aggregate((TBundleVersion)null, (latest, next) => latest == null || latest.CompareTo(next) < 0 ? next : latest);

            return bundles
                .Where(bundle => !bundle.Version.Equals(latestSdk));
        }
    }
}
