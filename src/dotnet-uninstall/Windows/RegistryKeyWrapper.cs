using System.IO;
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

            ParseVersionAndArch(displayName, bundleVersion, out var version, out var arch);

            return Bundle.From(version, arch, uninstallCommand);
        }

        private static void ParseVersionAndArch(string displayName, string bundleVersion, out BundleVersion version, out BundleArch arch)
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

            var buildNumberMatch = Regexes.BundleDisplayVersionStringRegex.Match(bundleVersion);
            var buildNumberString = buildNumberMatch.Groups[Regexes.BuildNumberGroupName].Value;
            var buildNumber = int.Parse(buildNumberString);

            switch (match.Groups[Regexes.TypeGroupName].Value)
            {
                case "SDK":
                    var sdkMinorString = match.Groups[Regexes.SdkMinorGroupName].Value;
                    var sdkMinor = int.Parse(sdkMinorString);

                    version = new SdkVersion(major, minor, sdkMinor, patch, buildNumber, preview, displayName);

                    break;
                case "Runtime":
                    version = new RuntimeVersion(major, minor, patch, buildNumber, preview, displayName);
                    break;
                default:
                    throw new InvalidDataException();
            }

            switch (archString)
            {
                case "x64": arch = BundleArch.X64; break;
                case "x86": arch = BundleArch.X86; break;
                case "arm32": arch = BundleArch.Arm32; break;
                default: throw new InvalidDataException();
            }
        }
    }
}
