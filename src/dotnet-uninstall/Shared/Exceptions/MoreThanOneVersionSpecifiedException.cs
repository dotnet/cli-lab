namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class MoreThanOneVersionSpecifiedException : DotNetUninstallException
    {
        public MoreThanOneVersionSpecifiedException() :
            base(LocalizableStrings.MoreThanOneVersionSpecifiedExceptionMessage)
        { }
    }
}
