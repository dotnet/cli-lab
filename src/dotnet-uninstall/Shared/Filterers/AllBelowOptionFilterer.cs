using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllBelowOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<ISdkInfo> Filter(string argValue, IEnumerable<ISdkInfo> sdks)
        {
            SdkVersion version = VersionRegex.ParseVersionString(argValue);
            return sdks.Where(sdk => sdk.Version.CompareTo(version) < 0);
        }
    }
}
