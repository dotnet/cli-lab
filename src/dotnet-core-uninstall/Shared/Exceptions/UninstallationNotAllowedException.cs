using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class UninstallationNotAllowedException : DotNetUninstallException
    {
        public UninstallationNotAllowedException() :
            base(string.Format(LocalizableStrings.UninstallNotAllowedExceptionFormat, VisualStudioSafeVersionsExtractor.UpperLimit.ToNormalizedString()))
        { }
    }
}
