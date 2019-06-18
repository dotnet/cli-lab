namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class SpecifiedVersionNotFoundException : DotNetUninstallException
    {
        public SpecifiedVersionNotFoundException(string versionString) :
            base(string.Format(Messages.SpecifiedVersionNotFoundExceptionMessageFormat, versionString))
        { }
    }
}
