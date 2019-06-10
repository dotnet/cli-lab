using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    class AllPreviewsOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<ISdkInfo> Filter(IEnumerable<ISdkInfo> sdks)
        {
            return sdks.Where(sdk => sdk.Version.Preview != null);
        }
    }
}
