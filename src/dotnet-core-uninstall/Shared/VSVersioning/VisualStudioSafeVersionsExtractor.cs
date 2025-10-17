// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning
{
    internal static class VisualStudioSafeVersionsExtractor
    {
        // The tool should not be used to uninstall any more recent versions of the sdk
        public static readonly SemanticVersion UpperLimit = new SemanticVersion(11, 0, 0);

        // Must keep one of each of these divisions to ensure Visual Studio works. 
        // Pairs are [inclusive, exclusive)
        private static readonly Dictionary<(SemanticVersion, SemanticVersion), string> WindowsVersionDivisionsToExplanation = new Dictionary<(SemanticVersion, SemanticVersion), string>
        {
            { (new SemanticVersion(1, 0, 0), new SemanticVersion(2, 0, 0)),  string.Format(LocalizableStrings.WindowsRequirementExplanationString, "") },
            { (new SemanticVersion(2, 0, 0), new SemanticVersion(2, 1, 300)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, "") },
            { (new SemanticVersion(2, 1, 300), new SemanticVersion(2, 1, 600)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2017") },
            { (new SemanticVersion(2, 1, 600), new SemanticVersion(2, 1, 900)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2019") },
            { (new SemanticVersion(2, 2, 100), new SemanticVersion(2, 2, 200)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2017") },
            { (new SemanticVersion(2, 2, 200), new SemanticVersion(2, 2, 500)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2019") },
            { (new SemanticVersion(3, 0, 100), new SemanticVersion(3, 1, 500)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2019") },
            { (new SemanticVersion(5, 0, 100), new SemanticVersion(5, 0, 600)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2019") },
            { (new SemanticVersion(6, 0, 100), new SemanticVersion(6, 0, 500)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2022") },
            { (new SemanticVersion(7, 0, 100), new SemanticVersion(7, 0, 500)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2022") },
            { (new SemanticVersion(8, 0, 100), new SemanticVersion(8, 0, 500)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2022") },
            { (new SemanticVersion(9, 0, 100), new SemanticVersion(9, 0, 400)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2022") },
            { (new SemanticVersion(10, 0, 0), new SemanticVersion(10, 0, 100)), string.Format(LocalizableStrings.WindowsRequirementExplanationString, "") },
            { (new SemanticVersion(10, 0, 100), UpperLimit), string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2026") }
        };

        private static (IDictionary<IEnumerable<Bundle>, string>, IEnumerable<Bundle>) ApplyWindowsVersionDivisions(IEnumerable<Bundle> bundleList)
        {
            var dividedBundles = new Dictionary<IEnumerable<Bundle>, string>();
            foreach (var (division, explanation) in WindowsVersionDivisionsToExplanation)
            {
                var bundlesInRange = bundleList.Where(bundle => bundle.Version is SdkVersion && 
                                                      division.Item1 <= bundle.Version.GetVersionWithoutTags() && bundle.Version.GetVersionWithoutTags() < division.Item2);
                bundleList = bundleList.Except(bundlesInRange);
                if (bundlesInRange.Count() > 0)
                {
                    dividedBundles.Add(bundlesInRange, explanation);
                }
            }

            return (dividedBundles, bundleList);
        }

        private static (IDictionary<IEnumerable<Bundle>, string>, IEnumerable<Bundle>) ApplyMacVersionDivisions(IEnumerable<Bundle> bundleList)
        {
            var bundlesAboveLimit = bundleList.Where(bundle => bundle.Version.SemVer >= UpperLimit);
            bundleList = bundleList.Except(bundlesAboveLimit);

            var dividedBundles = bundleList
                .Where(bundle => bundle.Version is RuntimeVersion)
                .GroupBy(bundle => bundle.Version.MajorMinor)
                .Select(pair => (pair as IEnumerable<Bundle>, LocalizableStrings.MacRuntimeRequirementExplanationString))
                .ToDictionary(key => key.Item1, value => value.Item2); 

            var sdks = bundleList.Where(bundle => bundle.Version is SdkVersion) ?? [];
            if (sdks.Any())
            {
                dividedBundles.Add(sdks, LocalizableStrings.MacSDKRequirementExplanationString);
            }

            var remainingBundles = bundleList
                .Where(bundle => bundle.Version is not RuntimeVersion && bundle.Version is not SdkVersion)
                .Concat(bundlesAboveLimit);
            return (dividedBundles, remainingBundles);
        }

        private static (IDictionary<IEnumerable<Bundle>, string>, IEnumerable<Bundle>) ApplyVersionDivisions(IEnumerable<Bundle> bundles, bool macOSPreserveVSSdks = false)
        {
            if (RuntimeInfo.RunningOnWindows)
            {
                return ApplyWindowsVersionDivisions(bundles);
            }
            else if (RuntimeInfo.RunningOnOSX && macOSPreserveVSSdks)
            {
                return ApplyMacVersionDivisions(bundles);
            }
            return (new Dictionary<IEnumerable<Bundle>, string>(), bundles);
        }

        public static IEnumerable<Bundle> GetUninstallableBundles(IEnumerable<Bundle> bundles, bool macOSPreserveVSSdks = false)
        {
            var protectedBundles = new List<Bundle>();
            var (bundlesByDivisions, remainingBundles) = ApplyVersionDivisions(bundles, macOSPreserveVSSdks);

            foreach (IEnumerable<Bundle> band in bundlesByDivisions.Keys)
            {
                protectedBundles.Add(band.Max());
            }

            protectedBundles = protectedBundles.Concat(remainingBundles.Where(bundle => bundle.Version.SemVer >= UpperLimit)).ToList();

            // Return all bundles that are not protectedBundles
            return bundles.Except(protectedBundles);
        }

        public static Dictionary<Bundle, string> GetReasonRequiredStrings(IEnumerable<Bundle> allBundles, bool macOSPreserveVSSdks = false)
        {
            var (bundlesByDivisions, remainingBundles) = ApplyVersionDivisions(allBundles, macOSPreserveVSSdks);

            var bundlesAboveUpperLimit = remainingBundles.Where(bundle => bundle.Version.SemVer >= UpperLimit);
            var requirementStringResults = remainingBundles.Except(bundlesAboveUpperLimit)
                .Select(bundle => (bundle, string.Empty))
                .Concat(bundlesAboveUpperLimit
                .Select(bundle => (bundle, string.Format(LocalizableStrings.UpperLimitRequirement, UpperLimit))));
            
            foreach (var division in bundlesByDivisions)
            {
                var requiredBundle = division.Key.Max();
                requirementStringResults = requirementStringResults.Concat([
                    (requiredBundle, division.Value),
                    ..division.Key
                        .Where(bundle => !bundle.Equals(requiredBundle))
                        .Select(bundle => (bundle, string.Empty))
                ]);
            }

            return requirementStringResults
                .GroupBy(pair => pair.bundle)
                .Select(group => group.First()) // Remove duplicates
                .OrderByDescending(pair => pair.bundle.DisplayName)
                .ToDictionary(i => i.bundle, i => i.Item2);
        }
    }
}
