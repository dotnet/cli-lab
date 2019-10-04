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
using System.Diagnostics;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using System.Threading;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;

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

        public static void Execute()
        {
            HandleVersionOption();

            var filtered = GetFilteredBundles(GetAllBundles());

            filtered = VSVersionHelper.CheckUninstallable(filtered);

            if (CommandLineConfigs.CommandLineParseResult.RootCommandResult.OptionResult(CommandLineConfigs.DryRunOption.Name) != null)
            {
                TryIt(filtered);
            }
            else if (CommandLineConfigs.CommandLineParseResult.RootCommandResult.OptionResult(CommandLineConfigs.YesOption.Name) != null)
            {
                if (!IsAdmin())
                {
                    throw new NotAdminException();
                }

                DoIt(filtered);
            }
            else
            {
                if (!IsAdmin())
                {
                    throw new NotAdminException();
                }

                AskIt(filtered);
            }
        }

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

        private static IEnumerable<Bundle> GetFilteredBundles(IEnumerable<Bundle> bundles)
        {
            var option = CommandLineConfigs.CommandLineParseResult.RootCommandResult.GetUninstallMainOption();
            var typeSelection = CommandLineConfigs.CommandLineParseResult.RootCommandResult.GetTypeSelection();
            var archSelection = CommandLineConfigs.CommandLineParseResult.RootCommandResult.GetArchSelection();

            if (option == null)
            {
                if (CommandLineConfigs.CommandLineParseResult.RootCommandResult.Tokens.Count == 0)
                {
                    throw new RequiredArgMissingForUninstallCommandException();
                }

                return OptionFilterers.UninstallNoOptionFilterer.Filter(
                    CommandLineConfigs.CommandLineParseResult.RootCommandResult.Tokens.Select(t => t.Value),
                    bundles,
                    typeSelection,
                    archSelection);
            }
            else
            {
                return OptionFilterers.OptionFiltererDictionary[option].Filter(
                    CommandLineConfigs.CommandLineParseResult,
                    option,
                    bundles,
                    typeSelection,
                    archSelection);
            }
        }

        private static void DoIt(IEnumerable<Bundle> bundles)
        {
            var verbosityLevel = CommandLineConfigs.CommandLineParseResult.RootCommandResult.GetVerbosityLevel();
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

        private static void TryIt(IEnumerable<Bundle> bundles)
        {
            var displayNames = string.Join("\n", bundles.Select(bundle => $"  {bundle.DisplayName}"));
            Console.WriteLine(string.Format(LocalizableStrings.DryRunOutputFormat, displayNames));
        }

        private static void AskIt(IEnumerable<Bundle> bundles)
        {
            var displayNames = string.Join("\n", bundles.Select(bundle => $"  {bundle.DisplayName}"));
            Console.Write(string.Format(LocalizableStrings.ConfirmationPromptOutputFormat, displayNames));

            var response = Console.ReadLine().Trim().ToUpper();

            if (response.Equals("Y"))
            {
                DoIt(bundles);
            }
            else if (response.Equals("N"))
            {
                return;
            }
            else
            {
                throw new ConfirmationPromptInvalidException();
            }
        }

        private static void HandleVersionOption()
        {
            if (CommandLineConfigs.CommandLineParseResult.RootCommandResult.OptionResult(CommandLineConfigs.VersionOption.Name) != null)
            {
                Console.WriteLine(_assemblyVersion.Value);
                Environment.Exit(0);
            }
        }
    }
}
