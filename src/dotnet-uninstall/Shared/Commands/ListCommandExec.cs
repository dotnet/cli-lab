using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using static Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class ListCommandExec
    {
        public static void Execute()
        {
            if (RuntimeInfo.RunningOnWindows)
            {
                Execute(SdkRegistryQuery.GetInstalledSdks());
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

        private static void Execute(IEnumerable<ISdkInfo> sdks)
        {
            foreach (var sdk in sdks)
            {
                Console.WriteLine(sdk.DisplayName);
            }
        }
    }
}
