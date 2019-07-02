using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class OptionsConflictException : DotNetUninstallException
    {
        public OptionsConflictException(IEnumerable<Option> options) :
            base(string.Format(LocalizableStrings.OptionsConflictExceptionMessageFormat, string.Join(", ", options.Select(option => $"--{option.Name}"))))
        { }
    }
}
