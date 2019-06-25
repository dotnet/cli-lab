using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllLowerPatchesOptionFilterer : NoArgFilterer
    {
        public override bool AcceptMultipleBundleTypes { get; } = true;

        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var highestVersions = new Dictionary<MajorMinorVersion, TBundleVersion>();

            foreach (var version in bundles
                .Select(bundle => bundle.Version))
            {
                if (highestVersions.TryGetValue(version.MajorMinor, out var highest))
                {
                    if (version.CompareTo(highest) > 0)
                    {
                        highestVersions[version.MajorMinor] = version;
                    }
                }
                else
                {
                    highestVersions.Add(version.MajorMinor, version);
                }
            }

            return bundles
                .Where(bundle =>
                {
                    var version = bundle.Version;
                    highestVersions.TryGetValue(version.MajorMinor, out var highest);
                    return version.CompareTo(highest) < 0;
                });
        }
    }
}
