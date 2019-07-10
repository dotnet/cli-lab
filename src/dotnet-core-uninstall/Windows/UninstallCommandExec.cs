using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class UninstallCommandExec
    {
        private const int UNINSTALL_TIMEOUT = 10 * 60 * 1000;

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        public static void ExecuteUninstallCommand(IEnumerable<Bundle> bundles)
        {
            if (!IsAdmin())
            {
                throw new NotAdminException();
            }

            var verbosityLevel = CommandLineConfigs.CommandLineParseResult.RootCommandResult.GetVerbosityLevel();
            var verbosityLogger = new VerbosityLogger(verbosityLevel);

            foreach (var bundle in bundles.ToList().AsReadOnly())
            {
                verbosityLogger.Log(VerbosityLevel.Normal, string.Format(LocalizableStrings.UninstallNormalVerbosityFormat, bundle.DisplayName));

                var args = ParseCommandToArgs(bundle.UninstallCommand, out var argc);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = args.First(),
                        Arguments = string.Join(" ", args.Skip(1)),
                        UseShellExecute = true,
                        Verb = "runas"
                    }
                };

                if (!process.Start() || !process.WaitForExit(UNINSTALL_TIMEOUT))
                {
                    throw new UninstallationFailedException(bundle.UninstallCommand);
                }

                if (process.ExitCode != 0)
                {
                    throw new UninstallationFailedException(bundle.UninstallCommand, process.ExitCode);
                }
            }
        }

        private static bool IsAdmin()
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<string> ParseCommandToArgs(string command, out int argc)
        {
            var argv = CommandLineToArgvW(command, out argc);

            if (argv == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            string[] args;
            try
            {
                args = new string[argc];

                for (var i = 0; i < argc; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }

            return args;
        }
    }
}
