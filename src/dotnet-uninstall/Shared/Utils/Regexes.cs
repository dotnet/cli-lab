using System.Text.RegularExpressions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Utils
{
    internal static class Regexes
    {
        public static readonly Regex DotNetCorePublisherRegex = new Regex(@"^Microsoft\sCorporation$");

        public static readonly Regex DotNetCoreSdkVersionRegex = new Regex(@"\d+\.\d+\.\d+(\s\-\spreview\d+)?");
        public static readonly Regex DotNetCoreRuntimeVersionRegex = new Regex(@"\d+\.\d+\.\d+(\sPreview\s\d+)?");

        public static readonly Regex DotNetCoreVersionExtractionRegex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)((\s\-\s|\-)preview(?<preview>\d+)|\sPreview\s(?<preview>\d+))?$");
        public static readonly string DotNetCoreVersionExtractionRegexMajorGroupName = "major";
        public static readonly string DotNetCoreVersionExtractionRegexMinorGroupName = "minor";
        public static readonly string DotNetCoreVersionExtractionRegexPatchGroupName = "patch";
        public static readonly string DotNetCoreVersionExtractionRegexPreviewGroupName = "preview";

        public static readonly Regex DotNetCoreMajorMinorExtractionRegex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)$");
        public static readonly string DotNetCoreMajorMinorExtractionRegexMajorGroupName = "major";
        public static readonly string DotNetCoreMajorMinorExtractionRegexMinorGroupName = "minor";

        public static readonly Regex DotNetCoreVersionRegex = new Regex(string.Format(
            @"{0}|{1}",
            DotNetCoreSdkVersionRegex.ToString(),
            DotNetCoreRuntimeVersionRegex.ToString()));

        public static readonly Regex DotNetCoreDisplayNameRegex = new Regex(string.Format(
            @"^Microsoft\s\.NET\sCore\s(SDK\s{0}|Runtime\s\-\s{1})\s\((x64|x86)\)$",
            DotNetCoreSdkVersionRegex.ToString(),
            DotNetCoreRuntimeVersionRegex.ToString()));
    }
}
