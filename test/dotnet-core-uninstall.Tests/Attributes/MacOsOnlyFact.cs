using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class MacOsOnlyFact : FactAttribute
    {
        public MacOsOnlyFact()
        {
            if (!RuntimeInfo.RunningOnOSX)
            {
                Skip = "Ignored on non-macOS platforms";
            }
        }
    }
}
