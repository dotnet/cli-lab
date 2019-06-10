namespace Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo
{
    internal interface ISdkInfo
    {
        SdkVersion Version { get; }
        string DisplayName { get; }
        string UninstallCommand { get; }
    }
}
