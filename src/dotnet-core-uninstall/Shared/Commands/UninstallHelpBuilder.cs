using System;
using System.CommandLine;
using System.CommandLine.Help;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    public class UninstallHelpBuilder : HelpBuilder
    {
        public UninstallHelpBuilder(IConsole console) : base(console) { }

        public override void Write(ICommand command)
        {
            base.Write(command);
            if (command.Name.Equals("dry-run") || command.Name.Equals("remove"))
            {
                Console.Out.Write(RuntimeInfo.RunningOnWindows ? LocalizableStrings.HelpExplainationParagraphWindows :
                    LocalizableStrings.HelpExplainationParagraphMac);
                Console.Out.Write(Environment.NewLine);
            }
        }
    }
}
