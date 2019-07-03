namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class VersionBeforeOptionException : DotNetUninstallException
    {
        public VersionBeforeOptionException(string option) :
            base(string.Format(LocalizableStrings.VersionBeforeOptionExceptionMessageFormat, option))
        { }
    }
}
