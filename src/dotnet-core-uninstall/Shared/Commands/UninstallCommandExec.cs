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
using System.IO;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using System.Threading;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class UninstallCommandExec
    {
        private const int UNINSTALL_TIMEOUT = 10 * 60 * 1000;

        [DllImport("libc")]
        private static extern uint getuid();

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

            var filtered = GetFilteredBundles(GetAllBundles());

            if (CommandLineConfigs.CommandLineParseResult.RootCommandResult.OptionResult(CommandLineConfigs.DoItOption.Name) != null)
            {
                DoIt(filtered);
            }
            else
            {
                TryIt(filtered);
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
            if (!IsAdmin())
            {
                throw new NotAdminException();
            }

            var verbosityLevel = CommandLineConfigs.CommandLineParseResult.RootCommandResult.GetVerbosityLevel();
            var verbosityLogger = new VerbosityLogger(verbosityLevel);

            var canceled = false;
            var cancelMutex = new Mutex();

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

                var args = ParseCommandToArgs(bundle.UninstallCommand);

                var process = new Process
                {
                    StartInfo = GetProcessStartInfo(args)
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

        private static ProcessStartInfo GetProcessStartInfo(IEnumerable<string> args)
        {
            return new ProcessStartInfo
            {
                FileName = args.First(),
                Arguments = string.Join(" ", args.Skip(1)),
                UseShellExecute = true,
                Verb = RuntimeInfo.RunningOnWindows ? "runas" : null
            };
        }

        private static IEnumerable<string> ParseCommandToArgs(string command)
        {
            return command
                .Split(' ')
                .Where(s => !string.IsNullOrWhiteSpace(s));
        }

        private static void TryIt(IEnumerable<Bundle> bundles)
        {
            Console.WriteLine(LocalizableStrings.DryRunStartMessage);

            foreach (var bundle in bundles)
            {
                Console.WriteLine(string.Format(LocalizableStrings.DryRunBundleFormat, bundle.DisplayName));
            }

            Console.WriteLine(LocalizableStrings.DryRunEndMessage);
            Console.WriteLine();
            Console.WriteLine(string.Format(
                LocalizableStrings.DryRunHowToDoItMessageFormat,
                $"{Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName)} {string.Join(" ", Environment.GetCommandLineArgs().Skip(1))}"));
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
