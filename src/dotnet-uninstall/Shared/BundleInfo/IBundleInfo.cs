namespace Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo
{
    internal interface IBundleInfo
    {
        BundleVersion Version { get; }
        string DisplayName { get; }
        string UninstallCommand { get; }
    }
}
