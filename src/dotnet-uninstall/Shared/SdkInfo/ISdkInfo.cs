namespace Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo
{
    interface ISdkInfo
    {
        SdkVersion Version { get; }
        string DisplayName { get; }
        string UninstallCommand { get; }
    }
}
