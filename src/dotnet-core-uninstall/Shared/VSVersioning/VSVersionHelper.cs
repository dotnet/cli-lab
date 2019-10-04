using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning
{
    /// <summary>
    /// Extracts VS versions and determine which sdk's are safe to uninstall
    /// </summary>
    internal static class VSVersionHelper
    {
        /// <summary>
        /// Divisions within version bands. Not inclusive, groupings: [2.1.000, 2.1.600) [2.1.600, 2.2.00) [2.0.00, 2.2.200) [2.2.200, 2.3.00)
        /// </summary>
        private static readonly SemanticVersion[] SpecialCaseDivisions = { new SemanticVersion(2, 1, 600), new SemanticVersion(2, 2, 200) };

        /// <summary>
        /// Assigns the uninstallAllowed flag to false for the highest sdkVersion in each band (Major.Minor) to ensure vs still functions properly
        /// Note that 2.1.5/2.1.6 and 2.2.1/2.2.2 count as band cutoffs 
        /// </summary>
        public static void AssignUninstallAllowed(IEnumerable<Bundle> bundles)
        {
            var bundlesByBand = SortByBand(bundles.Where(b => b.Version is SdkVersion).Select(b => b as Bundle<SdkVersion>));

            foreach (IEnumerable<Bundle> band in bundlesByBand)
            {
                band.Max().UninstallAllowed = false;
            }
        }

        /// <summary>
        /// Splits bundles into groups such that one of each group must be kept to protect visual studio: 
        ///     In general: 1 per Major.Minor band, except specialCaseDivision
        /// </summary>
        private static IEnumerable<IEnumerable<Bundle>> SortByBand(IEnumerable<Bundle<SdkVersion>> bundles)
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

        /// <summary>
        /// Splits bundleList based on the special case division
        /// </summary>
        private static IEnumerable<IEnumerable<Bundle>> DivideSpecialCases(IEnumerable<Bundle> bundleList, SemanticVersion division)
        {
            return bundleList.FirstOrDefault().Version.MajorMinor.Equals(new MajorMinorVersion(division.Major, division.Minor)) ?
                bundleList.GroupBy(bundle => bundle.Version.SemVer.Patch < division.Patch) as IEnumerable<IEnumerable<Bundle>> :
                new List<IEnumerable<Bundle>> { bundleList };
        }

        /// <summary>
        /// Check that the bundles are uninstallable, removes uninstallable bundles, and throws an error if not
        /// </summary>
        public static IEnumerable<Bundle> CheckUninstallable(IEnumerable<Bundle> bundles)
        {
            var uninstallablebundles = bundles.Where(item => item.UninstallAllowed);
            if (uninstallablebundles.Count() == 0)
            {
                // Invalid command, all bundles are uninstallable
                throw new UninstallationNotAllowedException(GetErrorStringList(bundles));
            }
            return uninstallablebundles;
        }

        /// <summary>
        /// Returns an error string listing uninstallable sdk versions
        /// </summary>
        public static string GetErrorStringList(IEnumerable<Bundle> bundles)
        {
            var errorString = string.Empty;
            foreach (Bundle b in bundles)
            {
                errorString += "\n\t" + b.DisplayName;
            }

            return errorString;
        }
    }
}
