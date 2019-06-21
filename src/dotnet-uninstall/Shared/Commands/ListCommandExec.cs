using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Windows;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class ListCommandExec
    {
        public static void Execute()
        {
            if (RuntimeInfo.RunningOnWindows)
            {
                Execute(RegistryQuery.GetInstalledBundles());
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                throw new NotImplementedException();
            }
            else if (RuntimeInfo.RunningOnLinux)
            {
                throw new LinuxNotSupportedException();
            }
        }

        private static void Execute(IEnumerable<Bundle> bundles)
        {
            var listCommandParseResult = CommandLineConfigs.ListCommand.Parse(Environment.GetCommandLineArgs());

            var typeSelection = listCommandParseResult.CommandResult.GetTypeSelection();
            var archSelection = listCommandParseResult.CommandResult.GetArchSelection();

            var stackView = new StackLayoutView();

            var filteredBundlesByArch = bundles.Where(bundle => (bundle.Arch & archSelection) > 0);

            if ((typeSelection & BundleType.Sdk) > 0)
            {
                var sdks = Bundle<SdkVersion>
                    .FilterWithSameBundleType(filteredBundlesByArch)
                    .OrderByDescending(sdk => sdk);

                stackView.Add(GetTableView(sdks,LocalizableStrings.ListCommandSdkHeader));
                stackView.Add(new ContentView(string.Empty));
            }

            if ((typeSelection & BundleType.Runtime) > 0)
            {
                var runtimes = Bundle<RuntimeVersion>
                    .FilterWithSameBundleType(filteredBundlesByArch)
                    .OrderByDescending(runtime => runtime);

                stackView.Add(GetTableView(runtimes,LocalizableStrings.ListCommandRuntimeHeader));
                stackView.Add(new ContentView(string.Empty));
            }

            stackView.Render(
                new ConsoleRenderer(new SystemConsole()),
                new Region(0, 0, Console.WindowWidth, Console.WindowHeight));
        }

        private static TableView<Bundle> GetTableView(IEnumerable<Bundle> bundles, string header)
        {
            var tableView = new TableView<Bundle>
            {
                Items = bundles.ToList().AsReadOnly()
            };

            tableView.AddColumn(bundle => $"  {bundle.Version.ToString()}", header);
            tableView.AddColumn(bundle => $"({bundle.Arch.ToString().ToLower()})", string.Empty);

            return tableView;
        }
    }
}
