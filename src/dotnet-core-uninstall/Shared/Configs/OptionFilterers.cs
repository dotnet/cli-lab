// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.Filterers;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs
{
    internal static class OptionFilterers
    {
        public static readonly IDictionary<Option, Filterer> OptionFiltererDictionary
            = new Dictionary<Option, Filterer>
            {
                {
                    CommandLineConfigs.UninstallAllOption,
                    new AllOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllLowerPatchesOption,
                    new AllLowerPatchesOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllButLatestOption,
                    new AllButLatestOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllButOption,
                    new AllButOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllBelowOption,
                    new AllBelowOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllPreviewsOption,
                    new AllPreviewsOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllPreviewsButLatestOption,
                    new AllPreviewsButLatestOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallMajorMinorOption,
                    new MajorMinorOptionFilterer()
                }
            };

        public static readonly ArgFilterer<IEnumerable<string>> UninstallNoOptionFilterer = new NoOptionFilterer();
    }
}
