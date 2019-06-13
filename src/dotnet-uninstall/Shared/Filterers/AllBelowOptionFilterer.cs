using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllBelowOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<Bundle> Filter(string argValue, IEnumerable<Bundle> bundles)
        {
            // TODO: add option handling for bundle type
            var version = SdkVersion.FromInput(argValue);
            return bundles.Where(bundle => bundle.Version.CompareTo(version) < 0);
        }
    }
}
