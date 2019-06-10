using System.Runtime.InteropServices;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Utils
{
    static class RuntimeInfo
    {
        public static readonly bool RunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static readonly bool RunningOnOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static readonly bool RunningOnLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
