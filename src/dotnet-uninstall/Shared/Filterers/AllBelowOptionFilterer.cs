using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllBelowOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<IBundleInfo> Filter(string argValue, IEnumerable<IBundleInfo> bundles)
        {
            // TODO: add option handling for bundle type
            var version = Regexes.ParseSdkVersionFromInput(argValue);
            return bundles.Where(bundle => bundle.Version.CompareTo(version) < 0);
        }
    }
}
