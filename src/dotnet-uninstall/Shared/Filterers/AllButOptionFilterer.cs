using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllButOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<string> argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var specifiedVersions = bundles
                .Select(bundle => bundle.Version)
                .Where(version => argValue.Contains(version.ToString()))
                .OrderBy(version => version);

            return bundles
                .Where(bundle => !specifiedVersions.Contains(bundle.Version));
        }
    }
}
