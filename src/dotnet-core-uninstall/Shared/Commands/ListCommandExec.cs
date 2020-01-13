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
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class ListCommandExec
    {
        private static IConsole SysConsole;
        private static Region Region;
        private static string[] Args;

        public static void Execute(IBundleCollector bundleCollector, IConsole console = null, Region region = null, string[] args = null)
        {
            SysConsole = console == null? new SystemConsole() : console;
            Region = region == null? new Region(0, 0, Console.WindowWidth, int.MaxValue) : region;
            Args = args == null? Environment.GetCommandLineArgs(): args;
            Execute(
                bundleCollector.GetAllInstalledBundles(),
                bundleCollector.GetSupportedBundleTypes());
        }

        private static void Execute(
            IEnumerable<Bundle> bundles,
            IEnumerable<BundleTypePrintInfo> supportedBundleTypes)
        {
            Console.WriteLine(RuntimeInfo.RunningOnWindows ? LocalizableStrings.WindowsListCommandOutput : LocalizableStrings.MacListCommandOutput);

            var uninstallMap = VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(bundles);

            var listCommandParseResult = CommandLineConfigs.ListCommand.Parse(Args);

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
                    var filteredWithStrings = uninstallMap.Where(pair => filteredBundlesByType.Contains(pair.Key)).ToDictionary(i => i.Key, i => i.Value);

                    stackView.Add(new ContentView(bundleType.Header));
                    stackView.Add(bundleType.GridViewGenerator.Invoke(filteredWithStrings, verbose));
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
                new ConsoleRenderer(SysConsole),
                Region);
        }
    }
}
