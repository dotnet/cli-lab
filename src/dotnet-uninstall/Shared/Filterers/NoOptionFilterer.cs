using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using static Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    class NoOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<ISdkInfo> Filter(IEnumerable<string> argValue, IEnumerable<ISdkInfo> sdks)
        {
            var uninstallVersions = argValue.Select(versionString => VersionRegex.ParseVersionString(versionString));
            var sdkVersions = sdks.Select(sdk => sdk.Version);

            foreach (var version in uninstallVersions)
            {
                if (!sdkVersions.Contains(version))
                {
                    throw new SpecifiedVersionNotFoundException(version);
                }
            }

            return sdks.Where(sdk => uninstallVersions.Contains(sdk.Version));
        }
    }
}
