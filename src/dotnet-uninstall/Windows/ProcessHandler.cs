using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    public static class ProcessHandler
    {
        private const int PROCESS_TIMEOUT = 5 * 60 * 1000;

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        internal static void ExecuteUninstallCommand(IEnumerable<Bundle> bundles)
        {
            foreach (var bundle in bundles.ToList().AsReadOnly())
            {
                var args = ParseCommand(bundle.UninstallCommand, out var argc).ToList();

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = args[0],
                        Arguments = string.Join(" ", args.Skip(1)),
                        Verb = "runas",
                        UseShellExecute = true
                    }
                };

                process.Start();

                if (!process.WaitForExit(PROCESS_TIMEOUT))
                {
                    throw new UninstallationFailedException(bundle.UninstallCommand);
                }
                else if (process.ExitCode != 0)
                {
                    throw new UninstallationFailedException(bundle.UninstallCommand, process.ExitCode);
                }
            }
        }

        private static IEnumerable<string> ParseCommand(string command, out int argc)
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
