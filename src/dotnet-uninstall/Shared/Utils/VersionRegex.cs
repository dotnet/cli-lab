using System.Text.RegularExpressions;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;
using static Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Utils
{
    internal static class VersionRegex
    {
        public static SdkVersion ParseVersionString(string versionString)
        {
            var regex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)((\s\-\s|\-)preview(?<preview>\d+))?$");
            var match = regex.Match(versionString);

            if (!match.Success)
            {
                throw new InvalidVersionStringException(versionString);
            }

            var versionMajorString = match.Groups["major"].Value;
            var versionMinorString = match.Groups["minor"].Value;
            var versionPatchString = match.Groups["patch"].Value;

            var versionPreviewString = match.Groups["preview"].Success ? match.Groups["preview"].Value : null;

            var versionMajor = int.Parse(versionMajorString);
            var versionMinor = int.Parse(versionMinorString);
            var versionPatch = int.Parse(versionPatchString);
            var versionPreview = versionPreviewString != null ? int.Parse(versionPreviewString) as int? : null;

            return new SdkVersion(versionMajor, versionMinor, versionPatch, versionPreview);
        }
    }
}
