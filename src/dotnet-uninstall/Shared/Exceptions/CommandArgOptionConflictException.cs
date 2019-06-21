using System.CommandLine;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class CommandArgOptionConflictException : DotNetUninstallException
    {
        public CommandArgOptionConflictException(Option option) :
            base(string.Format(LocalizableStrings.CommandArgOptionConflictExceptionMessageFormat, $"--{option.Name}"))
        { }
    }
}
