namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class MoreThanZeroVersionSpecifiedException : DotNetUninstallException
    {
        public MoreThanZeroVersionSpecifiedException() :
            base(LocalizableStrings.MoreThanZeroVersionSpecifiedExceptionMessage)
        { }
    }
}
