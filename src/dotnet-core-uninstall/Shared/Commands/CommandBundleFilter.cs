using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using System.Reflection;
using Microsoft.DotNet.Tools.Uninstall.MacOs;
using System.Linq;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class CommandBundleFilter
    {
        private static readonly Lazy<string> _assemblyVersion =
            new Lazy<string>(() =>
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                if (assemblyVersionAttribute == null)
                {
                    return assembly.GetName().Version.ToString();
                }
                else
                {
                    return assemblyVersionAttribute.InformationalVersion;
                }
            });

        private static IEnumerable<Bundle> GetAllBundles()
        {
            if (RuntimeInfo.RunningOnWindows)
            {
                return RegistryQuery.GetAllInstalledBundles();
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                return FileSystemExplorer.GetAllInstalledBundles();
            }
            else
            {
                throw new OperatingSystemNotSupportedException();
            }
        }

        public static IEnumerable<Bundle> GetFilteredBundles(IEnumerable<Bundle> bundles, ParseResult parseResult = null)
        {
            if (parseResult == null)
            {
                parseResult = CommandLineConfigs.CommandLineParseResult;
            }

            bundles = FilterRequiredBundles(bundles, parseResult.CommandResult.Tokens);

            var option = parseResult.CommandResult.GetUninstallMainOption();
            var typeSelection = parseResult.CommandResult.GetTypeSelection();
            var archSelection = parseResult.CommandResult.GetArchSelection();

            if (option == null)
            {
                if (parseResult.CommandResult.Tokens.Count == 0)
                {
                    throw new RequiredArgMissingForUninstallCommandException();
                }

                return OptionFilterers.UninstallNoOptionFilterer.Filter( 
                    parseResult.CommandResult.Tokens.Select(t => t.Value),
                    bundles,
                    typeSelection,
                    archSelection);
            }
            else
            {
                return OptionFilterers.OptionFiltererDictionary[option.Name].Filter(
                    parseResult,
                    option,
                    bundles,
                    typeSelection,
                    archSelection);
            }
        }

        public static IDictionary<Bundle, string> GetFilteredWithRequirementStrings()
        {
            var allBundles = GetAllBundles();
            var filteredBundles = GetFilteredBundles(allBundles);
            return VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(allBundles)
                    .Where(pair => filteredBundles.Contains(pair.Key))
                    .ToDictionary(i => i.Key, i => i.Value);
        }

        public static void HandleVersionOption()
        {
            if (CommandLineConfigs.CommandLineParseResult.CommandResult.OptionResult(CommandLineConfigs.VersionOption.Name) != null)
            {
                Console.WriteLine(_assemblyVersion.Value);
                Environment.Exit(0);
            }
        }

        private static IEnumerable<Bundle> FilterRequiredBundles(IEnumerable<Bundle> allBundles, IEnumerable<Token> tokens)
        {
            var explicitlyListedBundles = tokens
                .Where(token => SemanticVersion.TryParse(token.Value, out SemanticVersion semVerOut))
                .Select(token => SemanticVersion.Parse(token.Value));
            if (explicitlyListedBundles.Any(version => version >= VisualStudioSafeVersionsExtractor.UpperLimit))
            {
                throw new UninstallationNotAllowedException();
            }
            return VisualStudioSafeVersionsExtractor.GetUninstallableBundles(allBundles)
                .Concat(allBundles.Where(b => explicitlyListedBundles.Contains(b.Version.SemVer)))
                .Distinct()
                .ToList();
        }
    }
}
