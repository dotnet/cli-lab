namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class UninstallationFailedException : DotNetUninstallException
    {
        public UninstallationFailedException(string command) :
            base(string.Format(Messages.UninstallationFailedExceptionMessageFormat, command))
        { }

        public UninstallationFailedException(string command, int exitCode) :
            base(string.Format(Messages.UninstallationFailedExceptionWithExitCodeMessageFormat, command, exitCode))
        { }
    }
}
