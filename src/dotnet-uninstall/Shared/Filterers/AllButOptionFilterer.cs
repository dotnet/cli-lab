using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllButOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<ISdkInfo> Filter(IEnumerable<string> argValue, IEnumerable<ISdkInfo> sdks)
        {
            var excludedVersions = argValue.Select(versionString => new SdkVersion(versionString));
            return sdks.Where(sdk => !excludedVersions.Contains(sdk.Version));
        }
    }
}
