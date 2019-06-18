using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal abstract class DotNetUninstallException : Exception
    {
        public DotNetUninstallException(string message) : base(message) { }
    }
}
