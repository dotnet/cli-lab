using System.Text.RegularExpressions;
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

            var regex = new Regex(@"\d+\.\d+\.\d+(\s\-\spreview\d+)?");
            var versionString = regex.Match(DisplayName).Value;
            Version = VersionRegex.ParseVersionString(versionString);
        }
    }
}
