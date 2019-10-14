using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
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

        // The tool should not be used to uninstall any more recent versions of the sdk
        public static readonly SemanticVersion UpperLimit = RuntimeInfo.RunningOnWindows ? new SemanticVersion(3, 0, 0) : new SemanticVersion(5, 0, 0);

        public static IEnumerable<Bundle> GetUninstallableBundles(IEnumerable<Bundle> bundles)
        {
            var required = bundles.Where(b => b.Version.SemVer >= UpperLimit).ToList();
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

        public static Dictionary<Bundle, string> GetReasonRequiredStrings(IEnumerable<Bundle> allBundles, bool verbose)
        {
            var uninstallable = GetUninstallableBundles(allBundles);
            var required = allBundles.Where(b => !uninstallable.Contains(b));

            var ListCommandStringResults = uninstallable.Select(b => new KeyValuePair<Bundle, string>(b, string.Empty))
                        .ToDictionary(i => i.Key, i => i.Value);
            if (RuntimeInfo.RunningOnWindows && required.Where(b => b.Version.SemVer < SpecialCaseDivisions[0]).Count() > 0) 
            {
                ListCommandStringResults.Add(required.Where(b => b.Version.SemVer < SpecialCaseDivisions[0]).Max(),
                    verbose ? LocalizableStrings.VisualStudioRequirementLong : LocalizableStrings.VisualStudioRequirementShort);
            }
            foreach (var recentSdk in required.Where(b => b.Version.SemVer >= UpperLimit))
            {
                ListCommandStringResults.Add(recentSdk, string.Format(
                    verbose ? LocalizableStrings.UpperLimitRequirementLong : LocalizableStrings.UpperLimitRequirementShort,  UpperLimit));
            }

            return ListCommandStringResults.Concat(required
                .Where(b => !ListCommandStringResults.Keys.Contains(b))
                .Select(b => new KeyValuePair<Bundle, string>(b, string.Format(verbose ? LocalizableStrings.MajorMinorRequirementLong :
                    LocalizableStrings.MajorMinorRequirementShort, b.Version.Major, b.Version.Minor))))
                .OrderByDescending(pair => pair.Key)
                .ToDictionary(i => i.Key, i => i.Value);
        }
    }
}
