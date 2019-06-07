using System;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class ListCommand
    {
        internal static void Execute()
        {
            var installedDotnetCoreSdks = DotNetCoreSdkRegistryQuery.GetInstalledDotNetCoreSdks();

            foreach (var sdk in installedDotnetCoreSdks)
            {
                Console.WriteLine(sdk.GetValue("DisplayName"));
            }
        }
    }
}
