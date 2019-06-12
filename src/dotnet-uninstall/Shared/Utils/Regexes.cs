using System.Text.RegularExpressions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Utils
{
    internal static class Regexes
    {
        public static readonly string VersionMajorGroupName = "major";
        public static readonly string VersionMinorGroupName = "minor";
        public static readonly string VersionRegexPatchGroupName = "patch";
        public static readonly string VersionRegexPreviewGroupName = "preview";
        public static readonly string VersionRegexVersionGroupName = "version";

        private static readonly Regex _majorMinorRegex = new Regex(
            $@"(?<{VersionMajorGroupName}>\d+)\.(?<{VersionMinorGroupName}>\d+)");
        private static readonly Regex _dotNetCoreSdkVersionRegex = new Regex(
            $@"(?<{VersionRegexVersionGroupName}>{_majorMinorRegex.ToString()}\.(?<{VersionRegexPatchGroupName}>\d{{3,}})((\s\-\s|\-)preview(?<{VersionRegexPreviewGroupName}>\d+))?)");
        private static readonly Regex _dotNetCoreRuntimeVersionRegex = new Regex(
            $@"(?<{VersionRegexVersionGroupName}>{_majorMinorRegex.ToString()}\.(?<{VersionRegexPatchGroupName}>\d+)(\sPreview\s(?<{VersionRegexPreviewGroupName}>\d+))?)");

        public static readonly Regex DotNetCoreBundlePublisherRegex = new Regex(@"^Microsoft\sCorporation$");

        public static readonly Regex DotNetCoreBundleVersionRegex = new Regex(
            $@"(^{_dotNetCoreSdkVersionRegex.ToString()}$)|(^{_dotNetCoreRuntimeVersionRegex.ToString()}$)");

        public static readonly Regex DotNetCoreBundleMajorMinorRegex = new Regex(
            $@"^{_majorMinorRegex.ToString()}$");

        public static readonly Regex DotNetCoreBundleDisplayNameRegex = new Regex(
            $@"^Microsoft\s\.NET\sCore\s(SDK\s{_dotNetCoreSdkVersionRegex.ToString()}|Runtime\s\-\s{_dotNetCoreRuntimeVersionRegex.ToString()})\s\((x64|x86)\)$");
    }
}
