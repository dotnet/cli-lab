using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class MacOsOnlyTheory : TheoryAttribute
    {
        public MacOsOnlyTheory()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Skip = "Ignored on non-macOS platforms";
            }
        }
    }
}
