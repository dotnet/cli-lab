using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;
using Microsoft.Win32;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class SdkRegistryQuery
    {
        public static IEnumerable<ISdkInfo> GetInstalledSdks()
        {
            var uninstalls = Registry.LocalMachine
                .OpenSubKey("SOFTWARE")
                .OpenSubKey("WOW6432Node")
                .OpenSubKey("Microsoft")
                .OpenSubKey("Windows")
                .OpenSubKey("CurrentVersion")
                .OpenSubKey("Uninstall");

            var names = uninstalls.GetSubKeyNames();

            var sdks = names
                .Select(name => uninstalls.OpenSubKey(name))
                .Where(sdk => IsDotNetCore(sdk));

            return sdks.Select(sdk => new SdkRegistryKeyWrapper(sdk));
        }

        private static bool IsDotNetCore(RegistryKey rkey)
        {
            return IsDotNetCoreDisplayName(rkey.GetValue("DisplayName") as string)
                && IsDotNetCorePublisher(rkey.GetValue("Publisher") as string);
        }

        internal static bool IsDotNetCoreDisplayName(string displayName)
        {
            return displayName == null ?
                false :
                new Regex(@"^Microsoft\s\.NET\sCore\s(SDK|Runtime\s\-)\s\d+\.\d+\.\d+(\s\-\spreview\d+)?\s(\(x86\)|\(x64\))$")
                .IsMatch(displayName.ToString());
        }

        internal static bool IsDotNetCorePublisher(string publisher)
        {
            return publisher == null ?
                false :
                publisher.ToString().Equals("Microsoft Corporation");
        }
    }
}
