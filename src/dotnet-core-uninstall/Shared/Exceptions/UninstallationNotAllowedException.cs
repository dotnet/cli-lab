namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class UninstallationNotAllowedException : DotNetUninstallException
    {
        public UninstallationNotAllowedException(string sdkList) :
            base(string.Format(LocalizableStrings.UninstallNotAllowedExceptionFormat, sdkList))
        { }
    }
}
