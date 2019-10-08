using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning
{
    // Visual Studio versions 15.3 thru 15.9 (inclusive) requires an sdk with max version 2.1.5xx (inclusive)
    //                        16.0 requires anything less than 3.0.1xx
    // Must keep one of each Major.Minor band to ensure runtime works
    internal static class VisualStudioSafeVersionsExtractor
    {
        // Divisions within version bands. Not inclusive, groupings: [2.1.000, 2.1.600) [2.1.600, 2.2.00) [2.0.00, 2.2.200) [2.2.200, 2.3.00)
        private static readonly SemanticVersion[] SpecialCaseDivisions = { new SemanticVersion(2, 1, 600), new SemanticVersion(2, 2, 200) };

        public static IEnumerable<Bundle> GetUninstallableBundles(IEnumerable<Bundle> bundles)
        {
            var required = new List<Bundle>();
            var bundlesByBand = SortSdkBundlesByVersionBand(bundles
                .Where(b => b.Version is SdkVersion)
                .Select(b => b as Bundle<SdkVersion>));

            foreach (IEnumerable<Bundle> band in bundlesByBand)
            {
                required.Add(band.Max());
            }

            return bundles.Where(b => !required.Contains(b));
        }

        private static IEnumerable<IEnumerable<Bundle>> SortSdkBundlesByVersionBand(IEnumerable<Bundle<SdkVersion>> bundles)
        {
            var sortedBundles = new List<IEnumerable<Bundle>>() as IEnumerable<IEnumerable<Bundle>>;
            var majorMinorGroups = bundles.GroupBy(bundle => bundle.Version.MajorMinor);
            foreach (SemanticVersion specialCase in SpecialCaseDivisions)
            {
                sortedBundles = sortedBundles.Concat(
                    majorMinorGroups.Select(bundleList => DivideSpecialCases(bundleList, specialCase))
                    .SelectMany(lst => lst));
            }
            return sortedBundles;
        }

        private static IEnumerable<IEnumerable<Bundle>> DivideSpecialCases(IEnumerable<Bundle> bundleList, SemanticVersion division)
        {
            return bundleList.FirstOrDefault().Version.MajorMinor.Equals(new MajorMinorVersion(division.Major, division.Minor)) ?
                bundleList.GroupBy(bundle => bundle.Version.SemVer.Patch < division.Patch) as IEnumerable<IEnumerable<Bundle>> :
                new List<IEnumerable<Bundle>> { bundleList };
        }
    }
}
