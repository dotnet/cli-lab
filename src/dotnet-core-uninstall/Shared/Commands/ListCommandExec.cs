using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.MacOs;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;
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
                    RegistryQuery.GetAllInstalledBundles(),
                    Windows.SupportedBundleTypeConfigs.SupportedBundleTypes);
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                Execute(
                    FileSystemExplorer.GetAllInstalledBundles(),
                    MacOs.SupportedBundleTypeConfigs.SupportedBundleTypes);
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
            Console.WriteLine(RuntimeInfo.RunningOnWindows ? LocalizableStrings.WindowsListCommandOutput : LocalizableStrings.MacListCommandOutput);

            var listCommandParseResult = CommandLineConfigs.ListCommand.Parse(Environment.GetCommandLineArgs());

            var verbose = listCommandParseResult.CommandResult.GetVerbosityLevel().Equals(VerbosityLevel.Detailed) ||
                listCommandParseResult.CommandResult.GetVerbosityLevel().Equals(VerbosityLevel.Diagnostic);
            var typeSelection = listCommandParseResult.CommandResult.GetTypeSelection();
            var archSelection = listCommandParseResult.CommandResult.GetArchSelection();

            var stackView = new StackLayoutView();

            var filteredBundlesByArch = bundles.Where(bundle => archSelection.HasFlag(bundle.Arch));


            var footnotes = new List<string>();

            foreach (var bundleType in supportedBundleTypes)
            {
                if (typeSelection.HasFlag(bundleType.Type))
                {
                    var filteredBundlesByType = bundleType
                        .Filter(filteredBundlesByArch);

                    var uninstallMap = VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(filteredBundlesByType);

                    stackView.Add(new ContentView(bundleType.Header));
                    stackView.Add(bundleType.GridViewGenerator.Invoke(uninstallMap, verbose));
                    stackView.Add(new ContentView(string.Empty));

                    footnotes.AddRange(filteredBundlesByType
                        .Where(bundle => bundle.Version.HasFootnote)
                        .Select(bundle => bundle.Version.Footnote));
                }
            }

            foreach (var footnote in footnotes)
            {
                stackView.Add(new ContentView($"* {footnote}"));
            }

            if (footnotes.Count > 0)
            {
                stackView.Add(new ContentView(string.Empty));
            }

            stackView.Render(
                new ConsoleRenderer(new SystemConsole()),
                new Region(0, 0, Console.WindowWidth, int.MaxValue));
        }
    }
}
