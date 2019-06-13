using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class NoOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<BundleInfo.Bundle> Filter(IEnumerable<string> argValue, IEnumerable<BundleInfo.Bundle> bundles)
        {
            // TODO: add option handling for bundle type
            var uninstallVersions = argValue.Select(versionString => SdkVersion.FromInput(versionString));
            var bundleVersions = bundles.Select(bundle => bundle.Version);

            foreach (var version in uninstallVersions)
            {
                if (!bundleVersions.Contains(version))
                {
                    throw new SpecifiedVersionNotFoundException(version);
                }
            }

            return bundles.Where(bundle => uninstallVersions.Contains(bundle.Version));
        }
    }
}
