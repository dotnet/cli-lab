using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllPreviewsOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles)
        {
            return bundles.Where(bundle => bundle.Version.Preview != null);
        }
    }
}
