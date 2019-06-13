using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundle)
        {
            return bundle;
        }
    }
}
