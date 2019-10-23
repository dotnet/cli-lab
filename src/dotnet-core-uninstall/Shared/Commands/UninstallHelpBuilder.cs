using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    public class UninstallHelpBuilder : IHelpBuilder // TODO inherit from helpbuilder once package is updated
    {
        private readonly IConsole Console;

        public UninstallHelpBuilder(IConsole console)
        {
            Console = console;
        }

        public void Write(ICommand command)
        {
            Console.Out.WriteLine(RuntimeInfo.RunningOnWindows ? LocalizableStrings.HelpExplainationParagraphWindows :
                LocalizableStrings.HelpExplainationParagraphMac);
        }
    }
}
