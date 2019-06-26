using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllButOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<string> argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var versions = argValue
                .Select(value => BundleVersion.FromInput<TBundleVersion>(value));

            return bundles
                .Where(bundle => !versions.Contains(bundle.Version));
        }
    }
}
