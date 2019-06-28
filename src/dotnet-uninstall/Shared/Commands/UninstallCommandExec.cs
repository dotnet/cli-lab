using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using System.CommandLine;
using System.Reflection;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class UninstallCommandExec
    {
        private static readonly ParseResult _commandLineParseResult =
            CommandLineConfigs.UninstallRootCommand.Parse(Environment.GetCommandLineArgs());

        private static readonly Lazy<string> _assemblyVersion =
            new Lazy<string>(() => {
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

        public static void Execute()
        {
            HandleVersionOption();

            if (RuntimeInfo.RunningOnLinux)
            {
                throw new LinuxNotSupportedException();
            }

            if (RuntimeInfo.RunningOnWindows)
            {
                var filtered = GetFilteredBundles(RegistryQuery.GetInstalledBundles());
                Windows.UninstallCommandExec.ExecuteUninstallCommand(filtered);
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                throw new NotImplementedException();
            }
        }

        private static IEnumerable<Bundle> GetFilteredBundles(IEnumerable<Bundle> bundles)
        {
            var option = _commandLineParseResult.RootCommandResult.GetUninstallMainOption();
            var typeSelection = _commandLineParseResult.RootCommandResult.GetTypeSelection();
            var archSelection = _commandLineParseResult.RootCommandResult.GetArchSelection();

            if (option == null)
            {
                if (_commandLineParseResult.RootCommandResult.Arguments.Count == 0)
                {
                    throw new RequiredArgMissingForUninstallCommandException();
                }

                return OptionFilterers.UninstallNoOptionFilterer.Filter(_commandLineParseResult.RootCommandResult.Arguments, bundles, typeSelection, archSelection);
            }
            else
            {
                return OptionFilterers.OptionFiltererDictionary[option].Filter(_commandLineParseResult, option, bundles, typeSelection, archSelection);
            }
        }

        private static void HandleVersionOption()
        {
            if (_commandLineParseResult.HasOption(CommandLineConfigs.VersionOption))
            {
                Console.WriteLine(_assemblyVersion.Value);
                Environment.Exit(0);
            }
        }
    }
}
