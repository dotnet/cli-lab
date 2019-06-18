namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class InvalidInputVersionException : DotNetUninstallException
    {
        public InvalidInputVersionException(string versionString) :
            base(string.Format(Messages.InvalidInputVersionExceptionMessageFormat, versionString))
        { }
    }
}
