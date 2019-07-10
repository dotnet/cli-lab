namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class VerbosityLevelInvalidException : DotNetUninstallException
    {
        public VerbosityLevelInvalidException() :
            base(LocalizableStrings.VerbosityLevelInvalidExceptionMessage)
        { }
    }
}
