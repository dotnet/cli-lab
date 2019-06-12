using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.Win32;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal class RegistryKeyWrapper : IBundleInfo
    {
        public BundleVersion Version { get; }

        public string DisplayName { get; }

        public string UninstallCommand { get; }

        public RegistryKeyWrapper(RegistryKey registryKey)
        {
            DisplayName = registryKey.GetValue("DisplayName") as string;
            UninstallCommand = registryKey.GetValue("UninstallString") as string;
            Version = new BundleVersion(ExtractVersionString(DisplayName));
        }

        private static string ExtractVersionString(string displayName)
        {
            var match = Regexes.DotNetCoreBundleDisplayNameRegex.Match(displayName);

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
