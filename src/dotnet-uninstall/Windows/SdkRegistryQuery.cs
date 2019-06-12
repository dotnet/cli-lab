using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.Win32;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class RegistryQuery
    {
        public static IEnumerable<IBundleInfo> GetInstalledBundles()
        {
            var uninstalls = Registry.LocalMachine
                .OpenSubKey("SOFTWARE")
                .OpenSubKey("WOW6432Node")
                .OpenSubKey("Microsoft")
                .OpenSubKey("Windows")
                .OpenSubKey("CurrentVersion")
                .OpenSubKey("Uninstall");

            var names = uninstalls.GetSubKeyNames();

            var bundles = names
                .Select(name => uninstalls.OpenSubKey(name))
                .Where(bundle => IsDotNetCoreBundle(bundle));

            return bundles.Select(bundle => new RegistryKeyWrapper(bundle));
        }

        private static bool IsDotNetCoreBundle(RegistryKey registryKey)
        {
            return IsDotNetCoreBundleDisplayName(registryKey.GetValue("DisplayName") as string)
                && IsDotNetCoreBundlePublisher(registryKey.GetValue("Publisher") as string)
                && IsDotNetCoreBundleUninstaller(registryKey.GetValue("WindowsInstaller") as int?);
        }

        private static bool IsDotNetCoreBundleDisplayName(string displayName)
        {
            return displayName == null ?
                false :
                Regexes.DotNetCoreBundleDisplayNameRegex.IsMatch(displayName);
        }

        private static bool IsDotNetCoreBundlePublisher(string publisher)
        {
            return publisher == null ?
                false :
                Regexes.DotNetCoreBundlePublisherRegex.IsMatch(publisher);
        }

        private static bool IsDotNetCoreBundleUninstaller(int? windowsInstaller)
        {
            return windowsInstaller == null;
        }
    }
}
