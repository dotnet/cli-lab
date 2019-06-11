using System.Text.RegularExpressions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Utils
{
    internal static class Regexes
    {
        private static readonly Regex DotNetCoreMajorMinorRegex = new Regex(@"(?<major>\d+)\.(?<minor>\d+)");
        private static readonly Regex DotNetCoreSdkVersionRegex = new Regex(string.Format(
            @"(?<version>{0}\.(?<patch>\d{{3,}})((\s\-\s|\-)preview(?<preview>\d+))?)",
            DotNetCoreMajorMinorRegex.ToString()));
        private static readonly Regex DotNetCoreRuntimeVersionRegex = new Regex(string.Format(
            @"(?<version>{0}\.(?<patch>\d+)(\sPreview\s(?<preview>\d+))?)",
            DotNetCoreMajorMinorRegex.ToString()));

        public static readonly string DotNetCoreExtractionRegexMajorGroupName = "major";
        public static readonly string DotNetCoreExtractionRegexMinorGroupName = "minor";
        public static readonly string DotNetCoreExtractionRegexPatchGroupName = "patch";
        public static readonly string DotNetCoreExtractionRegexPreviewGroupName = "preview";
        public static readonly string DotNetCoreExtractionRegexVersionGroupName = "version";

        public static readonly Regex DotNetCorePublisherRegex = new Regex(@"^Microsoft\sCorporation$");

        public static readonly Regex DotNetCoreVersionExtractionRegex = new Regex(string.Format(
            @"(^{0}$)|(^{1}$)",
            DotNetCoreSdkVersionRegex.ToString(),
            DotNetCoreRuntimeVersionRegex.ToString()));

        public static readonly Regex DotNetCoreMajorMinorExtractionRegex = new Regex(string.Format(
            @"^{0}$",
            DotNetCoreMajorMinorRegex.ToString()));

        public static readonly Regex DotNetCoreDisplayNameExtractionRegex = new Regex(string.Format(
            @"^Microsoft\s\.NET\sCore\s(SDK\s{0}|Runtime\s\-\s{1})\s\((x64|x86)\)$",
            DotNetCoreSdkVersionRegex.ToString(),
            DotNetCoreRuntimeVersionRegex.ToString()));
    }
}
