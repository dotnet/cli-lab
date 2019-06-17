using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using System.CommandLine;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class UninstallCommandExec
    {
        public static void Execute()
        {
            if (RuntimeInfo.RunningOnWindows || RuntimeInfo.RunningOnOSX)
            {
                Execute(RegistryQuery.GetInstalledBundles());
            }
            else if (RuntimeInfo.RunningOnLinux)
            {
                throw new LinuxNotSupportedException();
            }
        }

        private static void Execute(IEnumerable<Bundle> bundles)
        {
            var commandLineParseResult = CommandLineConfigs.UninstallRootCommand.Parse(Environment.GetCommandLineArgs());

            var option = commandLineParseResult.RootCommandResult.GetUninstallMainOptions();
            var typeSelection = commandLineParseResult.RootCommandResult.GetTypeSelection();

            IEnumerable<Bundle> filteredBundles;
            if (option == null)
            {
                if (commandLineParseResult.RootCommandResult.Arguments.Count == 0)
                {
                    throw new RequiredArgMissingForUninstallCommandException();
                }

                filteredBundles = OptionFilterers.UninstallNoOptionFilterer.Filter(commandLineParseResult.RootCommandResult.Arguments, bundles, typeSelection);
            }
            else
            {
                filteredBundles = OptionFilterers.OptionFiltererDictionary[option].Filter(commandLineParseResult, option, bundles, typeSelection);
            }

            if (RuntimeInfo.RunningOnWindows)
            {
                ExecuteUninstallWindows(filteredBundles);
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                throw new NotImplementedException();
            }
        }

        private static void ExecuteUninstallWindows(IEnumerable<Bundle> bundles)
        {
            ProcessHandler.ExecuteUninstallCommand(bundles);
        }
    }
}
