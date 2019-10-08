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
                return RegistryQuery.GetInstalledBundles();
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                return FileSystemExplorer.GetInstalledBundles();
            }
            else
            {
                throw new OperatingSystemNotSupportedException();
            }
        }

        public static IEnumerable<Bundle> GetFilteredBundles()
        {
            var bundles = GetAllBundles();
            var option = CommandLineConfigs.CommandLineParseResult.CommandResult.GetUninstallMainOption();
            var typeSelection = CommandLineConfigs.CommandLineParseResult.CommandResult.GetTypeSelection();
            var archSelection = CommandLineConfigs.CommandLineParseResult.CommandResult.GetArchSelection();

            if (option == null)
            {
                if (CommandLineConfigs.CommandLineParseResult.CommandResult.Tokens.Count == 0)
                {
                    throw new RequiredArgMissingForUninstallCommandException();
                }

                return OptionFilterers.UninstallNoOptionFilterer.Filter(
                    CommandLineConfigs.CommandLineParseResult.CommandResult.Tokens.Select(t => t.Value),
                    bundles,
                    typeSelection,
                    archSelection);
            }
            else
            {
                return OptionFilterers.OptionFiltererDictionary[option.Name].Filter(
                    CommandLineConfigs.CommandLineParseResult,
                    option,
                    bundles,
                    typeSelection,
                    archSelection);
            }
        }

        public static void HandleVersionOption()
        {
            if (CommandLineConfigs.CommandLineParseResult.CommandResult.OptionResult(CommandLineConfigs.VersionOption.Name) != null)
            {
                Console.WriteLine(_assemblyVersion.Value);
                Environment.Exit(0);
            }
        }
    }
}
