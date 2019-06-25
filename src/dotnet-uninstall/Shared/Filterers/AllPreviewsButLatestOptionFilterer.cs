using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllPreviewsButLatestOptionFilterer : NoArgFilterer
    {
        public override bool AcceptMultipleBundleTypes { get; } = true;

        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var latestSdk = bundles
                .Where(bundle => bundle.Version.IsPrerelease)
                .Select(bundle => bundle.Version)
                .Aggregate((TBundleVersion)null, (latest, next) => latest == null || latest.CompareTo(next) < 0 ? next : latest);

            return bundles
                .Where(bundle => bundle.Version.IsPrerelease && !bundle.Version.Equals(latestSdk));
        }
    }
}
