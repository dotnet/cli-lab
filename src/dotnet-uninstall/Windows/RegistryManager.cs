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
                if (IsDotNetCoreSdk(uninstall))
                {
                    sdks.Add(uninstall);
                }
            }

            return sdks.ToArray();
        }

        private static bool IsDotNetCoreSdk(RegistryKey rkey)
        {
            return IsDotNetCoreSdkDisplayName(rkey.GetValue("DisplayName"))
                && IsDotNetCoreSdkPublisher(rkey.GetValue("Publisher"));
        }

        internal static bool IsDotNetCoreSdkDisplayName(object displayName)
        {
            return displayName == null ?
                false :
                new Regex(@"^Microsoft\s.NET\sCore\sSDK\s(\d+)\.(\d+)\.(\d+)\s(\(x86\)|\(x64\))$")
                .IsMatch(displayName.ToString());
        }

        internal static bool IsDotNetCoreSdkPublisher(object publisher)
        {
            return publisher == null ?
                false :
                publisher.ToString().Equals("Microsoft Corporation");
        }
    }
}
