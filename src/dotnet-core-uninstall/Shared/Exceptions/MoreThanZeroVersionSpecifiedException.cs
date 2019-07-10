namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class MoreThanZeroVersionSpecifiedException : DotNetUninstallException
    {
        public MoreThanZeroVersionSpecifiedException(string option) :
            base(string.Format(LocalizableStrings.MoreThanZeroVersionSpecifiedExceptionMessageFormat, option))
        { }
    }
}
