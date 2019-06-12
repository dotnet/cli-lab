using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllBelowOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<IBundleInfo> Filter(string argValue, IEnumerable<IBundleInfo> bundles)
        {
            var version = new BundleVersion(argValue);
            return bundles.Where(bundle => bundle.Version.CompareTo(version) < 0);
        }
    }
}
