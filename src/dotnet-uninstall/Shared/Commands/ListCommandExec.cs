using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
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

        private static void Execute(IEnumerable<IBundleInfo> bundles)
        {
            foreach (var bundle in bundles)
            {
                Console.WriteLine(bundle.DisplayName);
            }
        }
    }
}
