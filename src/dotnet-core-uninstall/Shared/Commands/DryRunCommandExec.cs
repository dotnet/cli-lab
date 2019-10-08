using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using System.Linq;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class DryRunCommandExec
    {
        public static void Execute()
        {
            CommandBundleFilter.HandleVersionOption();

            var filtered = CommandBundleFilter.GetFilteredBundles();
            TryIt(filtered);
        }

        private static void TryIt(IEnumerable<Bundle> bundles)
        {
            var displayNames = string.Join("\n", bundles.Select(bundle => $"  {bundle.DisplayName}"));
            Console.WriteLine(string.Format(LocalizableStrings.DryRunOutputFormat, displayNames));
        }
    }
}
