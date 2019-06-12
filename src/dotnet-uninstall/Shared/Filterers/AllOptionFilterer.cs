using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<IBundleInfo> Filter(IEnumerable<IBundleInfo> bundle)
        {
            return bundle;
        }
    }
}
