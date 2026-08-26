using System.Runtime.CompilerServices;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class WindowsOnlyTheory : TheoryAttribute
    {
        public WindowsOnlyTheory([CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = -1)
            : base(sourceFilePath, sourceLineNumber)
        {
            if (!RuntimeInfo.RunningOnWindows)
            {
                Skip = "Ignored on non-Windows platforms";
            }
        }
    }
}
