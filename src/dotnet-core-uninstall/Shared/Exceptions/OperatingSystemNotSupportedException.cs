namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class OperatingSystemNotSupportedException : DotNetUninstallException
    {
        public OperatingSystemNotSupportedException() :
            base(LocalizableStrings.OperatingSystemNotSupportedExceptionMessage)
        { }
    }
}
