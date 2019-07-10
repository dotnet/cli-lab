using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class SpecifiedVersionNotFoundException : DotNetUninstallException
    {
        public SpecifiedVersionNotFoundException(IEnumerable<BundleVersion> versions) :
            base(string.Format(
                LocalizableStrings.SpecifiedVersionNotFoundExceptionMessageFormat,
                string.Join(", ", versions.Select(version => version.ToString()))))
        { }
    }
}
