using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllLowerPatchesOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles)
        {
            var highestPatches = new Dictionary<Tuple<int, int>, int>();

            foreach (var bundle in bundles)
            {
                var majorMinor = new Tuple<int, int>(bundle.Version.Major, bundle.Version.Minor);

                if (highestPatches.TryGetValue(majorMinor, out var highestPatch))
                {
                    highestPatches[majorMinor] = bundle.Version.Patch;
                }
                else
                {
                    highestPatches.Add(majorMinor, bundle.Version.Patch);
                }
            }

            return bundles.Where(bundle =>
            {
                highestPatches.TryGetValue(new Tuple<int, int>(bundle.Version.Major, bundle.Version.Minor), out var highestPatch);
                return bundle.Version.Patch < highestPatch;
            });
        }
    }
}
