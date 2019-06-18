namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class BundleTypeMissingException : DotNetUninstallException
    {
        public BundleTypeMissingException() :
            base(Messages.BundleTypeNotSpecifiedExceptionMessage)
        { }
    }
}
