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
            var displayVersion = registryKey.GetValue("DisplayVersion") as string;
            var uninstallCommand = registryKey.GetValue("QuietUninstallString") as string;

            ParseVersionFromDisplayNameAndBundleVersion(displayName, displayVersion, out var version, out var arch);

            return new Bundle(version, arch, uninstallCommand);
        }

        private static void ParseVersionFromDisplayNameAndBundleVersion(string displayName, string displayVersion, out BundleVersion version, out BundleArch arch)
        {
            var match = Regexes.BundleDisplayNameRegex.Match(displayName);

            var majorString = match.Groups[Regexes.MajorGroupName].Value;
            var minorString = match.Groups[Regexes.MinorGroupName].Value;
            var patchString = match.Groups[Regexes.PatchGroupName].Value;
            var archString = match.Groups[Regexes.ArchGroupName].Value;

            var major = int.Parse(majorString);
            var minor = int.Parse(minorString);
            var patch = int.Parse(patchString);

            BundleVersion.PreviewVersion preview;
            if (match.Groups[Regexes.PreviewNumberGroupName].Success)
            {
                var previewNumber = match.Groups[Regexes.PreviewGroupName].Success ?
                    int.Parse(match.Groups[Regexes.PreviewNumberGroupName].Value) as int? :
                    null;

                var buildNumberMatch = Regexes.BundleDisplayVersionStringRegex.Match(displayVersion);
                var buildNumberString = buildNumberMatch.Groups[Regexes.BuildNumberGroupName].Value;
                var buildNumber = int.Parse(buildNumberString);

                preview = new BundleVersion.PreviewVersion(previewNumber, buildNumber);
            }
            else
            {
                preview = null;
            }

            switch (match.Groups[Regexes.TypeGroupName].Value)
            {
                case "SDK":
                    var sdkMinorString = match.Groups[Regexes.SdkMinorGroupName].Value;
                    var sdkMinor = int.Parse(sdkMinorString);

                    version = new SdkVersion(major, minor, sdkMinor, patch, preview);

                    break;
                case "Runtime":
                    version = new RuntimeVersion(major, minor, patch, preview);
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
