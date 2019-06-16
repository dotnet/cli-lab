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
            if (RuntimeInfo.RunningOnWindows)
            {
                Execute(RegistryQuery.GetInstalledBundles());
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                throw new NotImplementedException();
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

            ExecuteUninstall(filteredBundles);
        }

        private static void ExecuteUninstall(IEnumerable<Bundle> bundles)
        {
            foreach (var bundle in bundles)
            {
                // TODO: replace this
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(string.Format("Uninstalling: {0}", bundle.UninstallCommand));
                Console.ResetColor();
            }
        }
    }
}
