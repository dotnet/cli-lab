using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
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
            var sdks = bundles.Where(sdk => sdk.Version.Type == BundleType.Sdk);
            var runtimes = bundles.Where(sdk => sdk.Version.Type == BundleType.Runtime);

            Console.WriteLine(".NET Core SDKs:");
            foreach (var sdk in sdks)
            {
                Console.WriteLine($"\t{sdk.ToString()}");
            }

            Console.WriteLine();

            Console.WriteLine(".NET Core Runtimes:");
            foreach (var runtime in runtimes)
            {
                Console.WriteLine($"\t{runtime.ToString()}");
            }
        }
    }
}
