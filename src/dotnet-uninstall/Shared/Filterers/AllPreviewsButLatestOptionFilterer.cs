using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllPreviewsButLatestOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<IBundleInfo> Filter(IEnumerable<IBundleInfo> bundles)
        {
            BundleVersion latest = null;

            foreach (var bundle in bundles)
            {
                if (bundle.Version.Preview != null && (latest == null || latest.CompareTo(bundle.Version) < 0))
                {
                    latest = bundle.Version;
                }
            }

            return bundles.Where(bundle => !bundle.Version.Equals(latest));
        }
    }
}
