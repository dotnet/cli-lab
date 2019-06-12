using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllButOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<IBundleInfo> Filter(IEnumerable<string> argValue, IEnumerable<IBundleInfo> bundles)
        {
            var excludedVersions = argValue.Select(versionString => new BundleVersion(versionString));
            return bundles.Where(bundle => !excludedVersions.Contains(bundle.Version));
        }
    }
}
