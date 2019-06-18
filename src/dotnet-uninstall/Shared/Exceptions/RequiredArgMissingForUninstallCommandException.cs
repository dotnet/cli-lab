namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class RequiredArgMissingForUninstallCommandException : DotNetUninstallException
    {
        public RequiredArgMissingForUninstallCommandException() :
            base(Messages.RequiredArgMissingForUninstallCommandExceptionMessage)
        { }
    }
}
