using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.Win32;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class RegistryKeyWrapper
    {
        public static Bundle WrapRegistryKey(RegistryKey registryKey)
        {
            var displayName = registryKey.GetValue("DisplayName") as string;
            var uninstallCommand = registryKey.GetValue("QuietUninstallString") as string;
            var bundleCachePath = registryKey.GetValue("BundleCachePath") as string;

            ParseVersionAndArch(displayName, bundleCachePath, out var version, out var arch);

            return Bundle.From(version, arch, uninstallCommand, displayName);
        }

        private static void ParseVersionAndArch(string displayName, string bundleCachePath, out BundleVersion version, out BundleArch arch)
        {
            var match = Regexes.BundleDisplayNameRegex.Match(displayName);
            var cachePathMatch = Regexes.BundleCachePathRegex.Match(bundleCachePath);
            var archString = match.Groups[Regexes.ArchGroupName].Value;

            switch (match.Groups[Regexes.TypeGroupName].Value)
            {
                case "SDK": version = new SdkVersion(cachePathMatch.Groups[Regexes.VersionGroupName].Value); break;
                case "Runtime": version = new RuntimeVersion(cachePathMatch.Groups[Regexes.VersionGroupName].Value); break;
                default: throw new ArgumentException();
            }

            switch (archString)
            {
                case "x64": arch = BundleArch.X64; break;
                case "x86": arch = BundleArch.X86; break;
                default: throw new ArgumentException();
            }
        }
    }
}
