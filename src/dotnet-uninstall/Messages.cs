namespace Microsoft.DotNet.Tools.Uninstall
{
    internal static class Messages
    {
        public static readonly string UninstallNoOptionDescription = "Remove the .NET Core SDKs specified";
        public static readonly string UninstallNoOptionArgumentName = "SDK-VERSIONS";
        public static readonly string UninstallNoOptionArgumentDescription = ".NET Core SDK versions to uninstall";

        public static readonly string UninstallAllOptionDescription = "Remove all .NET Core SDKs installed globally on the machine";

        public static readonly string UninstallOptionsConflictMessage = "Please specify only one of the options";
    }
}
