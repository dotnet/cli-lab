// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using System.Linq;
using System.Diagnostics;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using System.Threading;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.DotNet.Tools.Uninstall.MacOs;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class UninstallCommandExec
    {
        private const int UNINSTALL_TIMEOUT = 10 * 60 * 1000;

        [DllImport("libc")]
        private static extern uint getuid();

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        public static void Execute(IBundleCollector bundleCollector)
        {
            CommandBundleFilter.HandleVersionOption();

            var filtered = CommandBundleFilter.GetFilteredWithRequirementStrings(bundleCollector);

            if (CommandLineConfigs.CommandLineParseResult.CommandResult.OptionResult(CommandLineConfigs.YesOption.Name) != null)
            {
                if (!IsAdmin())
                {
                    throw new NotAdminException();
                }

                DoIt(filtered.Keys);
            }
            else
            {
                if (!IsAdmin())
                {
                    throw new NotAdminException();
                }

                if (AskItAndReturnUserAnswer(filtered))
                {
                    if (AskWithWarningsForRequiredBundles(filtered))
                    {
                        DoIt(filtered.Keys);
                    }
                }
            }
        }

        private static void DoIt(IEnumerable<Bundle> bundles)
        {
            var verbosityLevel = CommandLineConfigs.CommandLineParseResult.CommandResult.GetVerbosityLevel();
            var verbosityLogger = new VerbosityLogger(verbosityLevel);

            var canceled = false;
            using var cancelMutex = new Mutex();

            var cancelProcessHandler = new ConsoleCancelEventHandler((sender, cancelArgs) =>
            {
                cancelMutex.WaitOne();

                try
                {
                    if (!canceled)
                    {
                        canceled = true;
                        Console.WriteLine(LocalizableStrings.CancelingMessage);
                    }

                    cancelArgs.Cancel = true;
                }
                finally
                {
                    cancelMutex.ReleaseMutex();
                }
            });

            foreach (var bundle in bundles.ToList().AsReadOnly())
            {
                verbosityLogger.Log(VerbosityLevel.Normal, string.Format(LocalizableStrings.UninstallNormalVerbosityFormat, bundle.DisplayName));

                using var process = new Process
                {
                    StartInfo = GetProcessStartInfo(bundle.UninstallCommand)
                };

                Console.CancelKeyPress += cancelProcessHandler;

                if (!process.Start() || !process.WaitForExit(UNINSTALL_TIMEOUT))
                {
                    throw new UninstallationFailedException(bundle.UninstallCommand);
                }

                if (process.ExitCode != 0)
                {
                    throw new UninstallationFailedException(bundle.UninstallCommand, process.ExitCode);
                }

                Console.CancelKeyPress -= cancelProcessHandler;

                cancelMutex.WaitOne();

                try
                {
                    if (canceled)
                    {
                        Environment.Exit(1);
                    }
                }
                finally
                {
                    cancelMutex.ReleaseMutex();
                }
            }
        }

        private static bool IsAdmin()
        {
            try
            {
                if (RuntimeInfo.RunningOnWindows)
                {
                    var identity = WindowsIdentity.GetCurrent();
                    var principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
                else if (RuntimeInfo.RunningOnOSX)
                {
                    return getuid() == 0;
                }
                else
                {
                    throw new OperatingSystemNotSupportedException();
                }
            }
            catch
            {
                return false;
            }
        }

        private static ProcessStartInfo GetProcessStartInfo(string command)
        {
            if (RuntimeInfo.RunningOnWindows)
            {
                var args = ParseCommandToArgs(command);

                return new ProcessStartInfo
                {
                    FileName = args.First(),
                    Arguments = string.Join(" ", args.Skip(1)),
                    UseShellExecute = true,
                    Verb = "runas"
                };
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                return new ProcessStartInfo
                {
                    FileName = "rm",
                    Arguments = $"-rf {command}",
                    UseShellExecute = true
                };
            }
            else
            {
                throw new OperatingSystemNotSupportedException();
            }
        }

        private static IEnumerable<string> ParseCommandToArgs(string command)
        {
            var argv = CommandLineToArgvW(command, out var argc);

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

        public static bool AskItAndReturnUserAnswer(IDictionary<Bundle, string> bundles, string userResponse = null)
        {
            var displayNames = string.Join("\n", bundles.Select(bundle => $"  {bundle.Key.DisplayName}"));
            Console.Write(string.Format(RuntimeInfo.RunningOnWindows ? LocalizableStrings.WindowsConfirmationPromptOutputFormat : 
                LocalizableStrings.MacConfirmationPromptOutputFormat, displayNames));

            var response = userResponse == null ? Console.ReadLine().Trim().ToUpper() : userResponse.ToUpper();

            if (response.Equals("Y") || response.Equals("YES"))
            {
                return true;
            }
            else if (response.Equals("N"))
            {
                return false;
            }
            else
            {
                throw new ConfirmationPromptInvalidException();
            }
        }

        public static bool AskWithWarningsForRequiredBundles(IDictionary<Bundle, string> bundles, string userResponse = null) 
        {
            var requiredBundles = bundles.Where(b => !b.Value.Equals(string.Empty));
            foreach (var pair in requiredBundles)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(string.Format(RuntimeInfo.RunningOnWindows ? LocalizableStrings.WindowsRequiredBundleConfirmationPromptOutputFormat : 
                    LocalizableStrings.MacRequiredBundleConfirmationPromptOutputFormat, pair.Key.DisplayName, pair.Value));
                Console.ResetColor();
                var response = userResponse == null ? Console.ReadLine().Trim().ToUpper() : userResponse.ToUpper();
                if (response.Equals("N"))
                {
                    return false ;
                }
                else if (!(response.Equals("Y") || response.Equals("YES")))
                {
                    throw new ConfirmationPromptInvalidException();
                }
            }

            return true;
        }
    }
}
