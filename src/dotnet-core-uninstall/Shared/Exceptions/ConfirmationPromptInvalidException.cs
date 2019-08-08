namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class ConfirmationPromptInvalidException : DotNetUninstallException
    {
        public ConfirmationPromptInvalidException() :
            base(LocalizableStrings.ConfirmationPromptInvalidExceptionMessage)
        { }
    }
}
