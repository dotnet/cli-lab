using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.MacOs;
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
                Execute(RegistryQuery.GetInstalledBundles(), bundles => Windows.ListCommandExec.GetGridView(bundles.ToList()));
            }
            else if (RuntimeInfo.RunningOnOSX)
            {
                Execute(FileSystemExplorer.GetInstalledBundles(), bundles => MacOs.ListCommandExec.GetGridView(bundles.ToList()));
            }
            else
            {
                throw new OperatingSystemNotSupportedException();
            }
        }

        private static void Execute(IEnumerable<Bundle> bundles, Func<IEnumerable<Bundle>, GridView> gridViewGetter)
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

                stackView.Add(new ContentView(LocalizableStrings.ListCommandSdkHeader));
                stackView.Add(gridViewGetter.Invoke(sdks.ToArray()));
                stackView.Add(new ContentView(string.Empty));
            }

            if ((typeSelection & BundleType.Runtime) > 0)
            {
                var runtimes = Bundle<RuntimeVersion>
                    .FilterWithSameBundleType(filteredBundlesByArch)
                    .OrderByDescending(runtime => runtime);

                stackView.Add(new ContentView(LocalizableStrings.ListCommandRuntimeHeader));
                stackView.Add(gridViewGetter.Invoke(runtimes.ToArray()));
                stackView.Add(new ContentView(string.Empty));
            }

            stackView.Render(
                new ConsoleRenderer(new SystemConsole()),
                new Region(0, 0, Console.WindowWidth, Console.WindowHeight));
        }
    }
}
