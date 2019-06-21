using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllLowerPatchesOptionFilterer : NoArgFilterer
    {
        public override bool AcceptMultipleBundleTypes { get; } = true;

        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var highestPatches = new Dictionary<Tuple<int, int, int>, int>();

            foreach (var version in bundles
                .Select(bundle => bundle.Version))
            {
                var key = GetVersionKey(version);

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

            return bundles
                .Where(bundle =>
                {
                    var version = bundle.Version;
                    var key = GetVersionKey(version);
                    highestPatches.TryGetValue(key, out var highestPatch);
                    return version.Patch < highestPatch;
                });
        }

        private Tuple<int, int, int> GetVersionKey<TBundleVersion>(TBundleVersion version) where TBundleVersion : BundleVersion
        {
            switch (version.Type)
            {
                case BundleType.Sdk:
                    return new Tuple<int, int, int>(version.Major, version.Minor, (version as SdkVersion).SdkMinor);
                case BundleType.Runtime:
                    return new Tuple<int, int, int>(version.Major, version.Minor, default);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
