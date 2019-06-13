using System.Text.RegularExpressions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Utils
{
    internal static class Regexes
    {
        public static readonly string MajorGroupName = "major";
        public static readonly string MinorGroupName = "minor";
        public static readonly string SdkMinorGroupName = "sdkMinor";
        public static readonly string PatchGroupName = "patch";
        public static readonly string PreviewGroupName = "preview";
        public static readonly string BuildGroupName = "build";
        public static readonly string TypeGroupName = "type";
        public static readonly string ArchGroupName = "arch";

        private static readonly Regex _majorMinorRegex = new Regex(
            $@"(?<{MajorGroupName}>\d+)\.(?<{MinorGroupName}>\d+)");
        private static readonly Regex _previewVersionNumberRegex = new Regex(
            $@"(?<{PreviewGroupName}>\d+)");
        private static readonly Regex _previewVersionSdkDisplayNameRegex = new Regex(
            $@"\s\-\spreview{_previewVersionNumberRegex.ToString()}");
        private static readonly Regex _previewVersionRuntimeDisplayNameRegex = new Regex(
            $@"\sPreview\s{_previewVersionNumberRegex.ToString()}");
        private static readonly Regex _previewVersionInputRegex = new Regex(
            $@"\-preview{_previewVersionNumberRegex.ToString()}-(?<{BuildGroupName}>\d+)");

        private static readonly string _sdkVersionBasicRegexFormat =
            $@"{_majorMinorRegex.ToString()}\.(?<{SdkMinorGroupName}>\d+)(?<{PatchGroupName}>\d{{{{2}}}})({{0}})?";
        private static readonly string _runtimeVersionBasicRegexFormat =
            $@"{_majorMinorRegex.ToString()}\.(?<{PatchGroupName}>\d+)({{0}})?";

        private static readonly Regex _sdkVersionDisplayNameRegex = new Regex(string.Format(
            _sdkVersionBasicRegexFormat,
            _previewVersionSdkDisplayNameRegex.ToString()));
        private static readonly Regex _runtimeVersionDisplayNameRegex = new Regex(string.Format(
            _runtimeVersionBasicRegexFormat,
            _previewVersionRuntimeDisplayNameRegex.ToString()));

        public static readonly Regex BundlePublisherRegex = new Regex(
            @"^Microsoft\sCorporation$");
        public static readonly Regex BundleMajorMinorRegex = new Regex(
            $@"^{_majorMinorRegex.ToString()}$");
        public static readonly Regex BundleDisplayNameRegex = new Regex(
            $@"^Microsoft\s\.NET\sCore\s((?<{TypeGroupName}>SDK)\s{_sdkVersionDisplayNameRegex.ToString()}|(?<{TypeGroupName}>Runtime)\s\-\s{_runtimeVersionDisplayNameRegex.ToString()})\s\((?<{ArchGroupName}>(x64|x86|arm32))\)$");
        public static readonly Regex BundleDisplayVersionStringRegex = new Regex(
            $@"^{_majorMinorRegex.ToString()}\.(?<{PatchGroupName}>\d+)\.(?<{BuildGroupName}>\d+)$");
        public static readonly Regex SdkVersionInputRegex = new Regex($@"^{string.Format(
            _sdkVersionBasicRegexFormat,
            _previewVersionInputRegex.ToString())}$");
        public static readonly Regex RuntimeVersionInputRegex = new Regex($@"^{string.Format(
            _runtimeVersionBasicRegexFormat,
            _previewVersionInputRegex.ToString())}$");
    }
}
