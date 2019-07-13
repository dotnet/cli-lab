using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.MacOs;
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
                Execute(
                    RegistryQuery.GetInstalledBundles(),
                    Windows.ListCommandExec.SupportedBundleTypes);
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                Execute(
                    FileSystemExplorer.GetInstalledBundles(),
                    MacOs.ListCommandExec.SupportedBundleTypes);
            }
            else
            {
                throw new OperatingSystemNotSupportedException();
            }
        }

        private static void Execute(
            IEnumerable<Bundle> bundles,
            IEnumerable<BundleTypePrintInfo> supportedBundleTypes)
        {
            var listCommandParseResult = CommandLineConfigs.ListCommand.Parse(Environment.GetCommandLineArgs());

            var typeSelection = listCommandParseResult.CommandResult.GetTypeSelection();
            var archSelection = listCommandParseResult.CommandResult.GetArchSelection();

            var stackView = new StackLayoutView();

            var filteredBundlesByArch = bundles.Where(bundle => archSelection.HasFlag(bundle.Arch));

            foreach (var bundleType in supportedBundleTypes)
            {
                if (typeSelection.HasFlag(bundleType.Type))
                {
                    var filteredBundlesByType = bundleType
                        .Filter(filteredBundlesByArch)
                        .OrderByDescending(bundle => bundle);

                    stackView.Add(new ContentView(bundleType.Header));
                    stackView.Add(bundleType.GridViewGenerator.Invoke(filteredBundlesByType.ToArray()));
                    stackView.Add(new ContentView(string.Empty));
                }
            }

            stackView.Render(
                new ConsoleRenderer(new SystemConsole()),
                new Region(0, 0, Console.WindowWidth, Console.WindowHeight));
        }
    }
}
