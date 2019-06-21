using System.CommandLine;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class OptionsConflictException : DotNetUninstallException
    {
        public OptionsConflictException(Option option1, Option option2) :
            base(string.Format(LocalizableStrings.OptionsConflictExceptionMessageFormat, $"--{option1.Name}", $"--{option2.Name}"))
        { }
    }
}
