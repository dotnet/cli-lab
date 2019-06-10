using System;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal static class Exceptions
    {
        public class LinuxNotSupportedException : Exception { }

        public class OptionsConflictException : Exception { }

        public class InvalidVersionStringException : Exception
        {
            public InvalidVersionStringException(string versionString) : base(versionString) { }
        }

        public class SpecifiedVersionNotFoundException : Exception
        {
            public SpecifiedVersionNotFoundException(SdkVersion version)
                : base(string.Format("{0}.{1}.{2}", version.Minor, version.Minor, version.Patch)) { }
        }
    }
}
