// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;

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
        public static readonly string VersionGroupName = "version";
        public static readonly string AuxVersionGroupName = "auxVersion";

        private static readonly Regex _majorMinorRegex = new Regex(
            $@"(?<{MajorGroupName}>\d+)\.(?<{MinorGroupName}>\d+)");
        private static readonly Regex _previewVersionNumberRegex = new Regex(
            $@"(\d+(\.\d+)*)?");
        private static readonly Regex _rcVersionNumberRegex = new Regex(
            $@"\d+");
        private static readonly Regex _buildNumberRegex = new Regex(
            $@"(?<{BuildGroupName}>\d+)");
        private static readonly Regex _archRegex = new Regex(
            $@"(?<{ArchGroupName}>\-?x64|x86)");

        private static readonly Regex _previewVersionSdkDisplayNameRegex = new Regex(
            $@"(?<{PreviewGroupName}>\s?\-\s?((preview|alpha)\.?{_previewVersionNumberRegex.ToString()}|rc{_rcVersionNumberRegex.ToString()}))");
        private static readonly Regex _previewVersionSdkCachePathRegex = new Regex(
            $@"(?<{PreviewGroupName}>\-((preview|alpha)\.?{_previewVersionNumberRegex.ToString()}|rc{_rcVersionNumberRegex.ToString()}(\.\d+)?)\-(?<{BuildGroupName}>\d+))");
        private static readonly Regex _previewVersionRuntimeCachePathRegex = new Regex(
            $@"(?<{PreviewGroupName}>\-(preview{_previewVersionNumberRegex.ToString()}\-{_buildNumberRegex.ToString()}\-\d+|rc{_rcVersionNumberRegex.ToString()}))");
        private static readonly Regex _previewVersionAspNetRuntimeCachePathRegex = new Regex(
            $@"(?<{PreviewGroupName}>\-(preview{_previewVersionNumberRegex.ToString()}(\.{_buildNumberRegex.ToString()}\.\d+|\-(final|{_buildNumberRegex.ToString()}(\-\d+)?))|rc{_rcVersionNumberRegex.ToString()}\-final))");
        private static readonly Regex _previewVersionHostingBundleCachePathRegex = new Regex(
            $@"(?<{PreviewGroupName}>\-(preview{_previewVersionNumberRegex.ToString()}(\.{_buildNumberRegex.ToString()}\.\d+|\-(final|{_buildNumberRegex.ToString()}(\-\d+)?))|rc{_rcVersionNumberRegex.ToString()}\-final))");

        private static readonly string _sdkVersionBasicRegexFormat =
            $@"(?<{VersionGroupName}>{_majorMinorRegex.ToString()}\.((?<{SdkMinorGroupName}>\d+)(?<{PatchGroupName}>\d{{{{2}}}})|(?<{PatchGroupName}>\d{{{{1,2}}}}))({{0}})?)";
        private static readonly string _notCapturedRuntimeVersionBasicRegexFormat =
            $@"{_majorMinorRegex.ToString()}\.(?<{PatchGroupName}>\d+)({{0}})?";
        private static readonly string _runtimeVersionBasicRegexFormat =
            $@"(?<{VersionGroupName}>{_notCapturedRuntimeVersionBasicRegexFormat})";
        private static readonly string _runtimeAuxVersionBasicRegexFormat =
            $@"(?<{AuxVersionGroupName}>{_notCapturedRuntimeVersionBasicRegexFormat})";
        private static readonly Regex _sdkVersionCachePathRegex = new Regex(string.Format(
            _sdkVersionBasicRegexFormat,
            _previewVersionSdkCachePathRegex.ToString()));
        private static readonly Regex _runtimeVersionCachePathRegex = new Regex(string.Format(
            _runtimeVersionBasicRegexFormat,
            _previewVersionRuntimeCachePathRegex.ToString()));
        private static readonly Regex _aspNetRuntimeVersionCachePathRegex = new Regex(string.Format(
            _runtimeVersionBasicRegexFormat,
            _previewVersionAspNetRuntimeCachePathRegex.ToString()));
        private static readonly Regex _hostingBundleVersionCachePathRegex = new Regex(string.Format(
            _runtimeVersionBasicRegexFormat,
            _previewVersionHostingBundleCachePathRegex.ToString()));
        private static readonly Regex _hostingBundleAuxVersionCachePathRegex = new Regex(string.Format(
            _runtimeAuxVersionBasicRegexFormat,
            _previewVersionHostingBundleCachePathRegex.ToString()));
        private static readonly Regex _sdkCachePathRegex = new Regex(
            $@"\\dotnet\-(?<{TypeGroupName}>sdk)\-{_sdkVersionCachePathRegex}\-win\-{_archRegex.ToString()}\.exe|\\dotnet\-dev\-win\-{_archRegex.ToString()}\.{_sdkVersionCachePathRegex.ToString()}\.exe");
        private static readonly Regex _runtimeCachePathRegex = new Regex(
            $@"\\dotnet\-(?<{TypeGroupName}>runtime)\-{_runtimeVersionCachePathRegex}\-win\-{_archRegex.ToString()}\.exe|\\dotnet\-win\-{_archRegex.ToString()}\.{_runtimeVersionCachePathRegex.ToString()}\.exe");
        private static readonly Regex _aspNetRuntimeCachePathRegex = new Regex(
            $@"\\(?<{TypeGroupName}>AspNetCore)\.{_aspNetRuntimeVersionCachePathRegex.ToString()}\.RuntimePackageStore_{_archRegex.ToString()}\.exe|\\(?<{TypeGroupName}>aspnetcore\-runtime)\-{_aspNetRuntimeVersionCachePathRegex.ToString()}\-win\-{_archRegex.ToString()}\.exe");
        private static readonly Regex _aspNetSharedFrameworkCachePathRegex = new Regex(
            $@"\\(?<{TypeGroupName}>AspNetCoreSharedFrameworkBundle)-{_archRegex.ToString()}\.exe");
        private static readonly Regex _hostingBundleCachePathRegex = new Regex(
            $@"\\DotNetCore\.({_hostingBundleAuxVersionCachePathRegex.ToString()}_)?{_hostingBundleVersionCachePathRegex.ToString()}\-(?<{TypeGroupName}>WindowsHosting)\.exe|\\dotnetcore\.{_hostingBundleAuxVersionCachePathRegex.ToString()}_{_hostingBundleVersionCachePathRegex.ToString()}\-(?<{TypeGroupName}>windowshosting)\.exe|\\dotnet\-(?<{TypeGroupName}>hosting)\-{_hostingBundleVersionCachePathRegex.ToString()}\-win\.exe");

        public static readonly Regex VersionDisplayNameRegex = new Regex(string.Format(
            _sdkVersionBasicRegexFormat,
            _previewVersionSdkDisplayNameRegex.ToString()));
        public static readonly Regex BundleMajorMinorRegex = new Regex(
            $@"^{_majorMinorRegex.ToString()}$");
        public static readonly Regex BundleCachePathRegex = new Regex(
            $@"({_sdkCachePathRegex.ToString()}|{_runtimeCachePathRegex.ToString()}|{_aspNetRuntimeCachePathRegex.ToString()}|{_aspNetSharedFrameworkCachePathRegex.ToString()}|{_hostingBundleCachePathRegex.ToString()})$");
    }
}
