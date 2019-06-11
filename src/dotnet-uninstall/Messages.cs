namespace Microsoft.DotNet.Tools.Uninstall
{
    internal static class Messages
    {
        public static readonly string ListCommandDescription = "List the .NET Core SDKs that are installed globally on the machine";

        public static readonly string UninstallNoOptionDescription = "Remove the .NET Core SDKs specified";
        public static readonly string UninstallNoOptionArgumentName = "SDK-VERSIONS";
        public static readonly string UninstallNoOptionArgumentDescription = "The specified .NET Core SDK versions to uninstall";

        public static readonly string UninstallAllOptionDescription = "Remove all .NET Core SDKs installed globally on the machine";

        public static readonly string UninstallAllLowerPatchesOptionDescription = "Remove .NET Core SDKs that have been replaced by higher patches";

        public static readonly string UninstallAllButLatestOptionDescription = "Remove all .NET Core SDKs, except the highest version";

        public static readonly string UninstallAllButOptionDescription = "Remove all .NET Core SDKs, except those listed";
        public static readonly string UninstallAllButOptionArgumentName = "SDK-VERSIONS";
        public static readonly string UninstallAllButOptionArgumentDescription = "The specified .NET Core SDK versions to remain";

        public static readonly string UninstallAllBelowOptionDescription = "Remove all .NET Core SDKs below specified version. The specified version will remain";
        public static readonly string UninstallAllBelowOptionArgumentName = "SDK-VERSION";
        public static readonly string UninstallAllBelowOptionArgumentDescription = "The specified .NET Core SDK version";

        public static readonly string UninstallAllPreviewsOptionDescription = "Remove all .NET Core Preview SDKs that are marked as previews";

        public static readonly string UninstallAllPreviewsButLatestOptionDescription = "Remove all .NET Core Preview SDKs that are marked as previews, except the latest preview";

        public static readonly string UninstallMajorMinorOptionDescription = "Remove .NET Core SDKs that match the specified major.minor version";
        public static readonly string UninstallMajorMinorOptionArgumentName = "MAJOR-MINOR";
        public static readonly string UninstallMajorMinorOptionArgumentDescription = "The specified major.minor version";

        public static readonly string UninstallVerbosityOptionDescription = "Set the MSBuild verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]";
        public static readonly string UninstallVerbosityOptionArgumentName = "VERBOSITY-LEVEL";
        public static readonly string UninstallVerbosityOptionArgumentDescription = "The specified MSBuild verbosity level";

        public static readonly string LinuxNotSupportedExceptionMessage = "The Linux operating systems are not supported";
        public static readonly string OptionsConflictExceptionMessage = "Please specify only one of the options";
        public static readonly string InvalidVersionStringExceptionMessageFormat = "The specified version is not valid: {0}";
        public static readonly string SpecifiedVersionNotFoundExceptionMessageFormat = "The specified version is not found: {0}";
    }
}
