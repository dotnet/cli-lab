using System.IO;
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
            Version = ParseVersionFromDisplayNameAndBundleVersion(DisplayName, registryKey.GetValue("DisplayVersion") as string);
        }

        private static BundleVersion ParseVersionFromDisplayNameAndBundleVersion(string displayName, string displayVersion)
        {
            var match = Regexes.BundleDisplayNameRegex.Match(displayName);

            var majorString = match.Groups[Regexes.MajorGroupName].Value;
            var minorString = match.Groups[Regexes.MinorGroupName].Value;
            var patchString = match.Groups[Regexes.PatchGroupName].Value;

            var major = int.Parse(majorString);
            var minor = int.Parse(minorString);
            var patch = int.Parse(patchString);

            PreviewVersion preview;
            if (match.Groups[Regexes.PreviewGroupName].Success)
            {
                var previewNumberString = match.Groups[Regexes.PreviewGroupName].Value;

                var buildNumberMatch = Regexes.BundleDisplayVersionStringRegex.Match(displayVersion);
                var buildNumberString = buildNumberMatch.Groups[Regexes.BuildGroupName].Value;

                var previewNumber = int.Parse(previewNumberString);
                var buildNumber = int.Parse(buildNumberString);

                preview = new PreviewVersion(previewNumber, buildNumber);
            }
            else
            {
                preview = null;
            }

            if (match.Groups[Regexes.TypeGroupName].Value == "SDK")
            {
                var sdkMinorString = match.Groups[Regexes.SdkMinorGroupName].Value;
                var sdkMinor = int.Parse(sdkMinorString);

                return new SdkVersion(major, minor, sdkMinor, patch, preview);
            }
            else if (match.Groups[Regexes.TypeGroupName].Value == "Runtime")
            {
                return new RuntimeVersion(major, minor, patch, preview);
            }
            else
            {
                throw new InvalidDataException();
            }
        }
    }
}
