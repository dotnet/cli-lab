using System;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class ListCommand
    {
        public static void Execute()
        {
            var installedDotnetCoreSdks = RegistryManager.GetInstalledDotnetCoreSdks();

            foreach (var sdk in installedDotnetCoreSdks)
            {
                Console.WriteLine(sdk.GetValue("DisplayName"));
            }
        }
    }
}
