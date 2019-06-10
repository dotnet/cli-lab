using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    class AllLowerPatchesOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<ISdkInfo> Filter(IEnumerable<ISdkInfo> sdks)
        {
            var highestPatches = new Dictionary<Tuple<int, int>, int>();

            foreach (var sdk in sdks)
            {
                var majorMinor = new Tuple<int, int>(sdk.Version.Major, sdk.Version.Minor);

                if (highestPatches.TryGetValue(majorMinor, out var highestPatch))
                {
                    highestPatches[majorMinor] = sdk.Version.Patch;
                }
                else
                {
                    highestPatches.Add(majorMinor, sdk.Version.Patch);
                }
            }

            return sdks.Where(sdk =>
            {
                highestPatches.TryGetValue(new Tuple<int, int>(sdk.Version.Major, sdk.Version.Minor), out var highestPatch);
                return sdk.Version.Patch < highestPatch;
            });
        }
    }
}
