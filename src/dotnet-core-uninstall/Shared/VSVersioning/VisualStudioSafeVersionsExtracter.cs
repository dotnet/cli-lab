using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning
{
    internal static class VisualStudioSafeVersionsExtracter
    {
        // Divisions within version bands. Not inclusive, groupings: [2.1.000, 2.1.600) [2.1.600, 2.2.00) [2.0.00, 2.2.200) [2.2.200, 2.3.00)
        private static readonly SemanticVersion[] SpecialCaseDivisions = { new SemanticVersion(2, 1, 600), new SemanticVersion(2, 2, 200) };

        public static void AssignUninstallAllowed(IEnumerable<Bundle> bundles)
        {
            var bundlesByBand = SortSdkBundlesByVersionBand(bundles
                .Where(b => b.Version is SdkVersion)
                .Select(b => b as Bundle<SdkVersion>));

            foreach (IEnumerable<Bundle> band in bundlesByBand)
            {
                band.Max().UninstallAllowed = false;
            }
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

        public static IEnumerable<Bundle> RemoveUninstallableBundles(IEnumerable<Bundle> bundles)
        {
            var uninstallablebundles = bundles.Where(item => item.UninstallAllowed);
            if (uninstallablebundles.Count() == 0)
            {
                throw new UninstallationNotAllowedException(GetInvalidSdkVersionsErrorMessage(bundles));
            }
            return uninstallablebundles;
        }

        private static string GetInvalidSdkVersionsErrorMessage(IEnumerable<Bundle> bundles)
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
