using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Windows;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class ListCommandExec
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
            var listCommandParseResult = CommandLineConfigs.ListCommand.Parse(Environment.GetCommandLineArgs());

            var typeSelection = listCommandParseResult.CommandResult.GetTypeSelection();
            var printed = false;

            var alignPrinter = new AlignPrinter<(string, string)>(new[] { AlignType.Left, AlignType.Left }, "    ", "  ");

            if ((typeSelection & BundleType.Sdk) > 0)
            {
                var sdks = Bundle<SdkVersion>.FilterWithSameBundleType(bundles).OrderByDescending(sdk => sdk.Version);

                Console.WriteLine(".NET Core SDKs:");
                alignPrinter.Print(sdks.Select(sdk => GetTupleForAlignPrinter(sdk)));

                printed = true;
            }

            if ((typeSelection & BundleType.Runtime) > 0)
            {
                if (printed)
                {
                    Console.WriteLine();
                }

                var runtimes = Bundle<RuntimeVersion>.FilterWithSameBundleType(bundles).OrderByDescending(runtime => runtime.Version);

                Console.WriteLine(".NET Core Runtimes:");
                alignPrinter.Print(runtimes.Select(runtime => GetTupleForAlignPrinter(runtime)));
            }
        }

        private static (string, string) GetTupleForAlignPrinter(Bundle bundle)
        {
            return (bundle.Version.ToString(), $"({bundle.Arch.ToString().ToLower()})");
        }
    }
}
