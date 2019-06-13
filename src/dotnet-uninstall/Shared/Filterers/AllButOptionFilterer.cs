using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllButOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<BundleInfo.Bundle> Filter(IEnumerable<string> argValue, IEnumerable<BundleInfo.Bundle> bundles)
        {
            // TODO: add option handling for bundle type
            var excludedVersions = argValue.Select(versionString => SdkVersion.FromInput(versionString));
            return bundles.Where(bundle => !excludedVersions.Contains(bundle.Version));
        }
    }
}
