using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class RegistryManager
    {
        public static RegistryKey [] GetInstalledDotnetCoreSdks()
        {
            var uninstalls = Registry.LocalMachine
                .OpenSubKey("SOFTWARE")
                .OpenSubKey("WOW6432Node")
                .OpenSubKey("Microsoft")
                .OpenSubKey("Windows")
                .OpenSubKey("CurrentVersion")
                .OpenSubKey("Uninstall");

            var names = uninstalls.GetSubKeyNames();
            var sdks = new List<RegistryKey>();

            foreach (var name in names)
            {
                var uninstall = uninstalls.OpenSubKey(name);
                if (IsDotnetCoreSdk(uninstall))
                {
                    sdks.Add(uninstall);
                }
            }

            return sdks.ToArray();
        }

        private static bool IsDotnetCoreSdk(RegistryKey rkey)
        {
            var displayName = rkey.GetValue("DisplayName");
            var publisher = rkey.GetValue("Publisher");

            if (displayName == null || publisher == null)
            {
                return false;
            }

            var regex = new Regex(@"^Microsoft\s.NET\sCore\sSDK\s(\d+)\.(\d+)\.(\d+)\s(\(x86\)|\(x64\))$");

            return regex.IsMatch(displayName.ToString())
                && publisher.ToString().Equals("Microsoft Corporation");
        }
    }
}
