namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class UserCancelationException : DotNetUninstallException
    {
        public UserCancelationException() :
            base(Messages.UserCancelationExceptionMessage)
        { }
    }
}
