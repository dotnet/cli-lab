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
            if (RuntimeInfo.RunningOnLinux)
            {
                throw new LinuxNotSupportedException();
            }

            var filtered = GetFilteredBundles(RegistryQuery.GetInstalledBundles());

            if (RuntimeInfo.RunningOnWindows)
            {
                Windows.UninstallCommandExec.ExecuteUninstallCommand(filtered);
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                throw new NotImplementedException();
            }
        }

        private static IEnumerable<Bundle> GetFilteredBundles(IEnumerable<Bundle> bundles)
        {
            var commandLineParseResult = CommandLineConfigs.UninstallRootCommand.Parse(Environment.GetCommandLineArgs());

            var option = commandLineParseResult.RootCommandResult.GetUninstallMainOption();
            var typeSelection = commandLineParseResult.RootCommandResult.GetTypeSelection();

            if (option == null)
            {
                if (commandLineParseResult.RootCommandResult.Arguments.Count == 0)
                {
                    throw new RequiredArgMissingForUninstallCommandException();
                }

                return OptionFilterers.UninstallNoOptionFilterer.Filter(commandLineParseResult.RootCommandResult.Arguments, bundles, typeSelection);
            }
            else
            {
                return OptionFilterers.OptionFiltererDictionary[option].Filter(commandLineParseResult, option, bundles, typeSelection);
            }
        }
    }
}
