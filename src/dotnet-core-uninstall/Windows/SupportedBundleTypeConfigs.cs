using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class SupportedBundleTypeConfigs
    {
        private static readonly Func<IDictionary<Bundle, string>, GridView> _gridViewGeneratorWithArch = bundles =>
        {
            var gridView = new GridView();

            gridView.SetColumns(Enumerable.Repeat(ColumnDefinition.SizeToContent(), 5).ToArray());
            gridView.SetRows(Enumerable.Repeat(RowDefinition.SizeToContent(), Math.Max(bundles.Count, 1)).ToArray());

            foreach (var (bundle, index) in bundles.Select((bundle, index) => (bundle, index)))
            {
                gridView.SetChild(new ContentView(string.Empty), 0, index);
                gridView.SetChild(new ContentView(bundle.Key.Version.ToStringWithAsterisk()), 1, index);
                gridView.SetChild(new ContentView(bundle.Key.Arch.ToString().ToLower()), 2, index);
                gridView.SetChild(new ContentView($"\"{bundle.Key.DisplayName}\""), 3, index);
                gridView.SetChild(new ContentView(bundle.Value.Equals(string.Empty) ? string.Empty : $"[{bundle.Value}]"), 4, index);
            }

            return gridView;
        };

        private static readonly Func<IDictionary<Bundle, string>, GridView> _gridViewGeneratorWithoutArch = bundles =>
        {
            var gridView = new GridView();

            gridView.SetColumns(Enumerable.Repeat(ColumnDefinition.SizeToContent(), 4).ToArray());
            gridView.SetRows(Enumerable.Repeat(RowDefinition.SizeToContent(), Math.Max(bundles.Count, 1)).ToArray());

            foreach (var (bundle, index) in bundles.Select((bundle, index) => (bundle, index)))
            {
                gridView.SetChild(new ContentView(string.Empty), 0, index);
                gridView.SetChild(new ContentView(bundle.Key.Version.ToStringWithAsterisk()), 1, index);
                gridView.SetChild(new ContentView($"\"{bundle.Key.DisplayName}\""), 2, index);
                gridView.SetChild(new ContentView(bundle.Value.Equals(string.Empty) ? string.Empty : $"[{bundle.Value}]"), 3, index);
            }

            return gridView;
        };

        public static readonly IEnumerable<BundleTypePrintInfo> SupportedBundleTypes =
            new BundleTypePrintInfo[]
            {
                new BundleTypePrintInfo<SdkVersion>(
                    LocalizableStrings.ListCommandSdkHeader,
                    _gridViewGeneratorWithArch,
                    CommandLineConfigs.SdkOptionName),

                new BundleTypePrintInfo<RuntimeVersion>(
                    LocalizableStrings.ListCommandRuntimeHeader,
                    _gridViewGeneratorWithArch,
                    CommandLineConfigs.RuntimeOptionName),

                new BundleTypePrintInfo<AspNetRuntimeVersion>(
                    LocalizableStrings.ListCommandAspNetRuntimeHeader,
                    _gridViewGeneratorWithArch,
                    CommandLineConfigs.AspNetRuntimeOptionName),

                new BundleTypePrintInfo<HostingBundleVersion>(
                    LocalizableStrings.ListCommandHostingBundleHeader,
                    _gridViewGeneratorWithoutArch,
                    CommandLineConfigs.HostingBundleOptionName)
            };
    }
}
