using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.Win32;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class RegistryKeyWrapper
    {
        public static Bundle WrapRegistryKey(RegistryKey registryKey)
        {
            var displayName = registryKey.GetValue("DisplayName") as string;
            var bundleVersion = registryKey.GetValue("BundleVersion") as string;
            var uninstallCommand = registryKey.GetValue("QuietUninstallString") as string;
            var bundleCachePath = registryKey.GetValue("BundleCachePath") as string;

            ParseVersionAndArch(displayName, bundleVersion, bundleCachePath, out var version, out var arch);

            return Bundle.From(version, arch, uninstallCommand);
        }

        private static void ParseVersionAndArch(string displayName, string bundleVersion, string bundleCachePath, out BundleVersion version, out BundleArch arch)
        {
            var match = Regexes.BundleDisplayNameRegex.Match(displayName);

            var majorString = match.Groups[Regexes.MajorGroupName].Value;
            var minorString = match.Groups[Regexes.MinorGroupName].Value;
            var patchString = match.Groups[Regexes.PatchGroupName].Value;
            var archString = match.Groups[Regexes.ArchGroupName].Value;

            var major = int.Parse(majorString);
            var minor = int.Parse(minorString);
            var patch = int.Parse(patchString);

            var preview = match.Groups[Regexes.PreviewGroupName].Success;

            var buildMatch = Regexes.BundleVersionRegex.Match(bundleVersion);
            var buildString = buildMatch.Groups[Regexes.BuildGroupName].Value;
            var build = buildString.Equals(string.Empty) ? default : int.Parse(buildString);

            var cachePathMatch = Regexes.BundleCachePathRegex.Match(bundleCachePath);
            var displayVersion = cachePathMatch.Groups[Regexes.VersionGroupName].Value;

            switch (match.Groups[Regexes.TypeGroupName].Value)
            {
                case "SDK":
                    var sdkMinorString = match.Groups[Regexes.SdkMinorGroupName].Value;
                    var sdkMinor = sdkMinorString.Equals(string.Empty) ? default : int.Parse(sdkMinorString);

                    version = new SdkVersion(major, minor, sdkMinor, patch, build, preview, displayVersion);

                    break;
                case "Runtime":
                    version = new RuntimeVersion(major, minor, patch, build, preview, displayVersion);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (archString)
            {
                case "x64": arch = BundleArch.X64; break;
                case "x86": arch = BundleArch.X86; break;
                case "arm32": arch = BundleArch.Arm32; break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
