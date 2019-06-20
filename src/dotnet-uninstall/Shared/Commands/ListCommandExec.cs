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
            var archSelection = listCommandParseResult.CommandResult.GetArchSelection();
            var printed = false;

            var filteredBundlesByArch = bundles.Where(bundle => (bundle.Arch & archSelection) > 0);

            if ((typeSelection & BundleType.Sdk) > 0)
            {
                var sdks = Bundle<SdkVersion>
                    .FilterWithSameBundleType(filteredBundlesByArch)
                    .OrderByDescending(sdk => sdk);

                Console.WriteLine(".NET Core SDKs:");
                foreach (var sdk in sdks)
                {
                    Console.WriteLine($"\t{sdk.ToString()}");
                }
                printed = true;
            }

            if ((typeSelection & BundleType.Runtime) > 0)
            {
                if (printed)
                {
                    Console.WriteLine();
                }

                var runtimes = Bundle<RuntimeVersion>
                    .FilterWithSameBundleType(filteredBundlesByArch)
                    .OrderByDescending(runtime => runtime);

                Console.WriteLine(".NET Core Runtimes:");
                foreach (var runtime in runtimes)
                {
                    Console.WriteLine($"\t{runtime.ToString()}");
                }
            }
        }
    }
}
