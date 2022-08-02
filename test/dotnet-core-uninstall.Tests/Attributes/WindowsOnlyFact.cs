using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class WindowsOnlyFact : FactAttribute
    {
        public WindowsOnlyFact()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Skip = "Ignored on non-Windows platforms";
            }
        }
    }
}
