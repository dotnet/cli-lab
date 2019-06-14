using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            if ((int)typeSelection < 1 || typeSelection > (BundleType.Sdk | BundleType.Runtime))
            {
                throw new ArgumentOutOfRangeException();
            }

            var sdkBundles = ((typeSelection & BundleType.Sdk) > 0) ?
                bundles.Where(bundle => bundle.Version is SdkVersion) :
                new List<Bundle>();

            var runtimeBundles = ((typeSelection & BundleType.Runtime) > 0) ?
                bundles.Where(bundle => bundle.Version is RuntimeVersion) :
                new List<Bundle>();

            return sdkBundles.Concat(runtimeBundles);
        }
    }
}
