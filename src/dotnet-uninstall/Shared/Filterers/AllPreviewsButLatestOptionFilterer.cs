using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllPreviewsButLatestOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            if ((int)typeSelection < 1 || typeSelection > (BundleType.Sdk | BundleType.Runtime))
            {
                throw new ArgumentOutOfRangeException();
            }

            IEnumerable<Bundle> sdkBundles;
            if ((typeSelection | BundleType.Sdk) > 0)
            {
                var latestSdk = bundles
                    .Where(bundle => bundle.Version is SdkVersion && bundle.Version.Preview)
                    .Select(bundle => bundle.Version as SdkVersion)
                    .Aggregate((SdkVersion)null, (latest, next) => latest.CompareTo(next) < 0 ? next : latest);

                sdkBundles = bundles
                    .Where(bundle => bundle.Version.Preview && !bundle.Version.Equals(latestSdk));
            }
            else
            {
                sdkBundles = new List<Bundle>();
            }

            IEnumerable<Bundle> runtimeBundles;
            if ((typeSelection | BundleType.Runtime) > 0)
            {
                var latestRuntime = bundles
                    .Where(bundle => bundle.Version is RuntimeVersion && bundle.Version.Preview)
                    .Select(bundle => bundle.Version as RuntimeVersion)
                    .Aggregate((RuntimeVersion)null, (latest, next) => latest.CompareTo(next) < 0 ? next : latest);

                runtimeBundles = bundles
                    .Where(bundle => bundle.Version.Preview && !bundle.Version.Equals(latestRuntime));
            }
            else
            {
                runtimeBundles = new List<Bundle>();
            }

            return sdkBundles.Concat(runtimeBundles);
        }
    }
}
