using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.Win32;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal class SdkRegistryKeyWrapper : ISdkInfo
    {
        public SdkVersion Version { get; }

        public string DisplayName { get; }

        public string UninstallCommand { get; }

        public SdkRegistryKeyWrapper(RegistryKey registryKey)
        {
            DisplayName = registryKey.GetValue("DisplayName") as string;
            UninstallCommand = registryKey.GetValue("UninstallString") as string;
            Version = new SdkVersion(ExtractVersionString(DisplayName));
        }

        private static string ExtractVersionString(string displayName)
        {
            var match = Regexes.DotNetCoreVersionRegex.Match(displayName);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                throw new InvalidVersionStringException(displayName);
            }
        }
    }
}
