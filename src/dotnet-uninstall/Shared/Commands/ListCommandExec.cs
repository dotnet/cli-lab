using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
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
            else
            {
                throw new OperatingSystemNotSupportedException();
            }
        }

        private static void Execute(IEnumerable<Bundle> bundles)
        {
            var listCommandParseResult = CommandLineConfigs.ListCommand.Parse(Environment.GetCommandLineArgs());

            var typeSelection = listCommandParseResult.CommandResult.GetTypeSelection();
            var archSelection = listCommandParseResult.CommandResult.GetArchSelection();

            var stackView = new StackLayoutView();

            var filteredBundlesByArch = bundles.Where(bundle => archSelection.HasFlag(bundle.Arch));

            if (typeSelection.HasFlag(BundleType.Sdk))
            {
                var sdks = Bundle<SdkVersion>
                    .FilterWithSameBundleType(filteredBundlesByArch)
                    .OrderByDescending(sdk => sdk);

                stackView.Add(new ContentView(LocalizableStrings.ListCommandSdkHeader));
                stackView.Add(GetGridView(sdks.ToArray()));
                stackView.Add(new ContentView(string.Empty));
            }

            if (typeSelection.HasFlag(BundleType.Runtime))
            {
                var runtimes = Bundle<RuntimeVersion>
                    .FilterWithSameBundleType(filteredBundlesByArch)
                    .OrderByDescending(runtime => runtime);

                stackView.Add(new ContentView(LocalizableStrings.ListCommandRuntimeHeader));
                stackView.Add(GetGridView(runtimes.ToArray()));
                stackView.Add(new ContentView(string.Empty));
            }

            stackView.Render(
                new ConsoleRenderer(new SystemConsole()),
                new Region(0, 0, Console.WindowWidth, Console.WindowHeight));
        }

        private static GridView GetGridView(IList<Bundle> bundles)
        {
            var gridView = new GridView();

            gridView.SetColumns(Enumerable.Repeat(ColumnDefinition.SizeToContent(), 3).ToArray());
            gridView.SetRows(Enumerable.Repeat(RowDefinition.SizeToContent(), Math.Max(bundles.Count, 1)).ToArray());

            foreach (var (bundle, index) in bundles.Select((bundle, index) => (bundle, index)))
            {
                gridView.SetChild(new ContentView(string.Empty), 0, index);
                gridView.SetChild(new ContentView(bundle.Version.ToString()), 1, index);
                gridView.SetChild(new ContentView($"\"{bundle.DisplayName}\""), 2, index);
            }

            return gridView;
        }
    }
}
