namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class UninstallationFailedException : DotNetUninstallException
    {
        public UninstallationFailedException(string command) :
            base(string.Format(LocalizableStrings.UninstallationFailedExceptionMessageFormat, command))
        { }

        public UninstallationFailedException(string command, int exitCode) :
            base(string.Format(LocalizableStrings.UninstallationFailedExceptionWithExitCodeMessageFormat, command, exitCode))
        { }
    }
}
