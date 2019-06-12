using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllButOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<IBundleInfo> Filter(IEnumerable<string> argValue, IEnumerable<IBundleInfo> bundles)
        {
            // TODO: add option handling for bundle type
            var excludedVersions = argValue.Select(versionString => Regexes.ParseSdkVersionFromInput(versionString));
            return bundles.Where(bundle => !excludedVersions.Contains(bundle.Version));
        }
    }
}
