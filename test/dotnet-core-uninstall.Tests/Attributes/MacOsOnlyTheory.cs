using System.Runtime.CompilerServices;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class MacOsOnlyTheory : TheoryAttribute
    {
        public MacOsOnlyTheory([CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = -1)
            : base(sourceFilePath, sourceLineNumber)
        {
            if (!RuntimeInfo.RunningOnOSX)
            {
                Skip = "Ignored on non-macOS platforms";
            }
        }
    }
}
