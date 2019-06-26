namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class NotAdminException : DotNetUninstallException
    {
        public NotAdminException() :
            base(LocalizableStrings.NotAdminExceptionMessage)
        { }
    }
}
