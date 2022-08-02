using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class WindowsOnlyTheory : TheoryAttribute
    {
        public WindowsOnlyTheory()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Skip = "Ignored on non-Windows platforms";
            }
        }
    }
}
