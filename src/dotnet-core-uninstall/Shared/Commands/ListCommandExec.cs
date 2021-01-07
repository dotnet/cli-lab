using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
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
        public static void Execute(IBundleCollector bundleCollector)
        {
            Execute(
                bundleCollector.GetAllInstalledBundles(),
                bundleCollector.GetSupportedBundleTypes());
        }

        private static void Execute(
            IEnumerable<Bundle> bundles,
            IEnumerable<BundleTypePrintInfo> supportedBundleTypes)
        {
            Console.WriteLine(RuntimeInfo.RunningOnWindows ? LocalizableStrings.WindowsListCommandOutput : LocalizableStrings.MacListCommandOutput);

            var listCommandParseResult = CommandLineConfigs.ListCommand.Parse(Environment.GetCommandLineArgs());
            var verbose = listCommandParseResult.CommandResult.GetVerbosityLevel().Equals(VerbosityLevel.Detailed) ||
                listCommandParseResult.CommandResult.GetVerbosityLevel().Equals(VerbosityLevel.Diagnostic);

            var sortedBundles = GetFilteredBundlesWithRequirements(bundles, supportedBundleTypes, listCommandParseResult);

            var stackView = new StackLayoutView();
            var footnotes = new List<string>();

            foreach ((var bundleType, var filteredBundles) in sortedBundles)
            {
                stackView.Add(new ContentView(bundleType.Header));
                stackView.Add(bundleType.GridViewGenerator.Invoke(filteredBundles, verbose));
                stackView.Add(new ContentView(string.Empty));

                footnotes.AddRange(filteredBundles
                    .Where(bundle => bundle.Key.Version.HasFootnote)
                    .Select(bundle => bundle.Key.Version.Footnote));
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
                new Region(0, 0, int.MaxValue, int.MaxValue));
        }

        public static Dictionary<BundleTypePrintInfo, Dictionary<Bundle, string>> GetFilteredBundlesWithRequirements(
            IEnumerable<Bundle> bundles,
            IEnumerable<BundleTypePrintInfo> supportedBundleTypes, 
            ParseResult parseResult)
        {
            var uninstallMap = VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(bundles);

            var typeSelection = parseResult.GetTypeSelection();
            var archSelection = parseResult.GetArchSelection();

            var filteredBundlesByArch = bundles.Where(bundle => archSelection.HasFlag(bundle.Arch));

            return supportedBundleTypes.Where(type => typeSelection.HasFlag(type.Type))
                                       .Select(type => new KeyValuePair<BundleTypePrintInfo, Dictionary<Bundle, string>>(type,
                                            uninstallMap.Where(pair => type.Filter(filteredBundlesByArch).Contains(pair.Key)).ToDictionary(i => i.Key, i => i.Value)))
                                       .ToDictionary(i => i.Key, i => i.Value);
        }
    }
}
