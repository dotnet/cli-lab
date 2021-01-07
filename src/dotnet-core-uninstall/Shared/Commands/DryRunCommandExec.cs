// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.MacOs;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal static class DryRunCommandExec
    {
        public static void Execute(IBundleCollector bundleCollector)
        {
            CommandBundleFilter.HandleVersionOption();

            var filtered = CommandBundleFilter.GetFilteredWithRequirementStrings(bundleCollector);
            TryIt(filtered);
        }

        private static void TryIt(IDictionary<Bundle, string> bundles)
        {
            var displayNames = string.Join("\n", bundles.Select(bundle => $"  {bundle.Key.DisplayName}"));
            Console.WriteLine(string.Format(RuntimeInfo.RunningOnWindows ? 
                LocalizableStrings.WindowsDryRunOutputFormat : LocalizableStrings.MacDryRunOutputFormat, displayNames));

            foreach (var pair in bundles.Where(b => !b.Value.Equals(string.Empty)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(string.Format(RuntimeInfo.RunningOnWindows ? LocalizableStrings.WindowsRequiredBundleConfirmationPromptWarningFormat : 
                    LocalizableStrings.MacRequiredBundleConfirmationPromptWarningFormat, pair.Key.DisplayName, pair.Value));
                Console.ResetColor();
            }
        }
    }
}
