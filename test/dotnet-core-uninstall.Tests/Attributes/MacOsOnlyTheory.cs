using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class MacOsOnlyTheory : TheoryAttribute
    {
        public MacOsOnlyTheory()
        {
            if (!RuntimeInfo.RunningOnOSX)
            {
                Skip = "Ignored on non-macOS platforms";
            }
        }
    }
}
