using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllLowerPatchesOptionFilterer : NoArgFilterer
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
                var highestPatches = new Dictionary<Tuple<int, int, int>, int>();

                foreach (var version in bundles
                    .Select(bundle => bundle.Version as SdkVersion)
                    .Where(version => version is SdkVersion))
                {
                    var key = new Tuple<int, int, int>(version.Major, version.Minor, version.SdkMinor);

                    if (highestPatches.TryGetValue(key, out var highestPatch))
                    {
                        if (version.Patch > highestPatch)
                        {
                            highestPatches[key] = version.Patch;
                        }
                    }
                    else
                    {
                        highestPatches.Add(key, version.Patch);
                    }
                }

                sdkBundles = bundles
                    .Where(bundle => bundle.Version is SdkVersion)
                    .Where(bundle =>
                    {
                        var version = bundle.Version as SdkVersion;
                        var key = new Tuple<int, int, int>(version.Major, version.Minor, version.SdkMinor);
                        highestPatches.TryGetValue(key, out var highestPatch);
                        return version.Patch < highestPatch;
                    });
            }
            else
            {
                sdkBundles = new List<Bundle>();
            }

            IEnumerable<Bundle> runtimeBundles;
            if ((typeSelection | BundleType.Runtime) > 0)
            {
                var highestPatches = new Dictionary<Tuple<int, int>, int>();

                foreach (var version in bundles
                    .Select(bundle => bundle.Version as RuntimeVersion)
                    .Where(version => version is RuntimeVersion))
                {
                    var key = new Tuple<int, int>(version.Major, version.Minor);

                    if (highestPatches.TryGetValue(key, out var highestPatch))
                    {
                        if (version.Patch > highestPatch)
                        {
                            highestPatches[key] = version.Patch;
                        }
                    }
                    else
                    {
                        highestPatches.Add(key, version.Patch);
                    }
                }

                runtimeBundles = bundles
                    .Where(bundle => bundle.Version is RuntimeVersion)
                    .Where(bundle =>
                    {
                        var version = bundle.Version as RuntimeVersion;
                        var key = new Tuple<int, int>(version.Major, version.Minor);
                        highestPatches.TryGetValue(key, out var highestPatch);
                        return version.Patch < highestPatch;
                    });
            }
            else
            {
                runtimeBundles = new List<Bundle>();
            }

            return sdkBundles.Concat(runtimeBundles);
        }
    }
}
