using System.Collections.Generic;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class BundleTypeMissingException : DotNetUninstallException
    {
        public BundleTypeMissingException(IEnumerable<string> options) :
            base(string.Format(
                LocalizableStrings.BundleTypeMissingExceptionMessage,
                string.Join(", ", options)))
        { }
    }
}
