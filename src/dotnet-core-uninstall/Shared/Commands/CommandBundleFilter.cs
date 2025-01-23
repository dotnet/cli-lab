// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using System.Reflection;
using Microsoft.DotNet.Tools.Uninstall.MacOs;
using System.Linq;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;
using NuGet.Versioning;
using System.CommandLine.Parsing;
using System.CommandLine.Binding;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class CommandBundleFilter
    {
        public static IEnumerable<Bundle> GetFilteredBundles(IEnumerable<Bundle> allBundles, ParseResult parseResult)
        {
            var option = parseResult.CommandResult.GetUninstallMainOption();
            var typeSelection = parseResult.GetTypeSelection();
            var archSelection = parseResult.GetArchSelection();
            var macOSPreserveVSSdks = parseResult.ValueForOption<bool>(CommandLineConfigs.MacOSPreserveVSSdksOption);
            var bundles = allBundles;

            if (option == null)
            {
                if (parseResult.CommandResult.Tokens.Count == 0)
                {
                    throw new RequiredArgMissingForUninstallCommandException();
                }

                bundles = OptionFilterers.UninstallNoOptionFilterer.Filter(
                    parseResult.CommandResult.Tokens.Select(t => t.Value),
                    bundles,
                    typeSelection,
                    archSelection);
            }
            else
            {
                bundles = OptionFilterers.OptionFiltererDictionary[option].Filter(
                    parseResult,
                    option,
                    bundles,
                    typeSelection,
                    archSelection);
            }

            if (parseResult.FindResultFor(CommandLineConfigs.ForceOption) == null)
            {
                bundles = FilterRequiredBundles(allBundles, parseResult.CommandResult.Tokens, macOSPreserveVSSdks).Intersect(bundles);
            }
            if (bundles.Any(bundle => bundle.Version.SemVer >= VisualStudioSafeVersionsExtractor.UpperLimit))
            {
                throw new UninstallationNotAllowedException();
            }
            return bundles;
        }

        public static IDictionary<Bundle, string> GetFilteredWithRequirementStrings(IBundleCollector bundleCollector, ParseResult parseResult)
        {
            var allBundles = bundleCollector.GetAllInstalledBundles();
            var filteredBundles = GetFilteredBundles(allBundles, parseResult);
            var macOSPreserveVSSdks = parseResult.ValueForOption<bool>(CommandLineConfigs.MacOSPreserveVSSdksOption);
            return VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(allBundles, macOSPreserveVSSdks)
                    .Where(pair => filteredBundles.Contains(pair.Key))
                    .ToDictionary(i => i.Key, i => i.Value);
        }

        private static IEnumerable<Bundle> FilterRequiredBundles(IEnumerable<Bundle> allBundles, IEnumerable<Token> tokens, bool macOSPreserveVSSdks)
        {
            var explicitlyListedBundles = tokens
                .Where(token => SemanticVersion.TryParse(token.Value, out SemanticVersion semVerOut))
                .Select(token => SemanticVersion.Parse(token.Value));
            return VisualStudioSafeVersionsExtractor.GetUninstallableBundles(allBundles, macOSPreserveVSSdks)
                .Concat(allBundles.Where(b => explicitlyListedBundles.Contains(b.Version.SemVer)))
                .Distinct()
                .ToList();
        }
    }
}
