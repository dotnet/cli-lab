using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal static class ListCommandExec
    {
        public static readonly IEnumerable<BundleTypePrintInfo> SupportedBundleTypes =
            new BundleTypePrintInfo[]
            {
                new BundleTypePrintInfo<SdkVersion>(LocalizableStrings.ListCommandSdkHeader),
                new BundleTypePrintInfo<RuntimeVersion>(LocalizableStrings.ListCommandRuntimeHeader),
                new BundleTypePrintInfo<AspNetRuntimeVersion>(LocalizableStrings.ListCommandAspNetRuntimeHeader),
                new BundleTypePrintInfo<HostingBundleVersion>(LocalizableStrings.ListCommandHostingBundleHeader)
            };

        public static GridView GetGridView(IList<Bundle> bundles)
        {
            var gridView = new GridView();

            gridView.SetColumns(Enumerable.Repeat(ColumnDefinition.SizeToContent(), 4).ToArray());
            gridView.SetRows(Enumerable.Repeat(RowDefinition.SizeToContent(), Math.Max(bundles.Count, 1)).ToArray());

            foreach (var (bundle, index) in bundles.Select((bundle, index) => (bundle, index)))
            {
                gridView.SetChild(new ContentView(string.Empty), 0, index);
                gridView.SetChild(new ContentView(bundle.Version.ToString()), 1, index);
                gridView.SetChild(new ContentView(bundle.Arch.ToString().ToLower()), 2, index);
                gridView.SetChild(new ContentView($"\"{bundle.DisplayName}\""), 3, index);
            }

            return gridView;
        }
    }
}
