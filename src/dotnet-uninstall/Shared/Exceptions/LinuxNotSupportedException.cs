namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class LinuxNotSupportedException : DotNetUninstallException
    {
        public LinuxNotSupportedException() :
            base(Messages.LinuxNotSupportedExceptionMessage)
        { }
    }
}
