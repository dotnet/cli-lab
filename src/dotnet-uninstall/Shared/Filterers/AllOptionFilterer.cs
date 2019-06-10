using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    class AllOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<ISdkInfo> Filter(IEnumerable<ISdkInfo> sdks)
        {
            return sdks;
        }
    }
}
