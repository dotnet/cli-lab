using System;
using System.Collections.Generic;
using System.CommandLine;
using Microsoft.Win32;
using static Microsoft.DotNet.Tools.Uninstall.Shared.UninstallCommand<Microsoft.Win32.RegistryKey>;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class UninstallCommand
    {
        internal static void Execute(ParseResult parseResult)
        {
            try
            {
                var installedSdks = DotNetCoreSdkRegistryQuery.GetInstalledDotNetCoreSdks();
                var selectedSdks = SelectSdks(installedSdks, parseResult);
                ExecuteUninstallCommands(selectedSdks);
            }
            catch (OptionsConflictException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Messages.UninstallOptionsConflict);
                Console.ResetColor();
            }
        }

        private static void ExecuteUninstallCommands(IEnumerable<RegistryKey> sdks)
        {
            // TODO: replace this
            foreach (var sdk in sdks)
            {
                Console.WriteLine(sdk.GetValue("UninstallString").ToString());
            }
        }
    }
}
