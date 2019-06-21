namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class ElevationFailedException : DotNetUninstallException
    {
        public ElevationFailedException() :
            base(LocalizableStrings.ElevationFailedExceptionMessage)
        { }
    }
}
