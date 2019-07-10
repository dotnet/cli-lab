namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class MoreThanOneVersionSpecifiedException : DotNetUninstallException
    {
        public MoreThanOneVersionSpecifiedException(string option) :
            base(string.Format(LocalizableStrings.MoreThanOneVersionSpecifiedExceptionMessageFormat, option))
        { }
    }
}
