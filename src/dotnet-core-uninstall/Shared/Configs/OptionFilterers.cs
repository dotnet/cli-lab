using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.Filterers;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs
{
    internal static class OptionFilterers
    {
        public static readonly IDictionary<string, Filterer> OptionFiltererDictionary
            = new Dictionary<string, Filterer>
            {
                {
                    CommandLineConfigs.UninstallAllOption.Name,
                    new AllOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllLowerPatchesOption.Name,
                    new AllLowerPatchesOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllButLatestOption.Name,
                    new AllButLatestOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllButOption.Name,
                    new AllButOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllBelowOption.Name,
                    new AllBelowOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllPreviewsOption.Name,
                    new AllPreviewsOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallAllPreviewsButLatestOption.Name,
                    new AllPreviewsButLatestOptionFilterer()
                },
                {
                    CommandLineConfigs.UninstallMajorMinorOption.Name,
                    new MajorMinorOptionFilterer()
                }
            };

        public static readonly ArgFilterer<IEnumerable<string>> UninstallNoOptionFilterer = new NoOptionFilterer();
    }
}
