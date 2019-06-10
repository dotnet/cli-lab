using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    class AllPreviewsButLatestOptionFilterer : NoArgFilterer
    {
        public override IEnumerable<ISdkInfo> Filter(IEnumerable<ISdkInfo> sdks)
        {
            SdkVersion latest = null;

            foreach (var sdk in sdks)
            {
                if (sdk.Version.Preview != null && (latest == null || latest.CompareTo(sdk.Version) < 0))
                {
                    latest = sdk.Version;
                }
            }

            return sdks.Where(sdk => !sdk.Version.Equals(latest));
        }
    }
}
