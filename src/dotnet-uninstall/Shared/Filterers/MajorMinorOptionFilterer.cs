using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class MajorMinorOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<ISdkInfo> Filter(string argValue, IEnumerable<ISdkInfo> sdks)
        {
            var match = Regexes.DotNetCoreMajorMinorExtractionRegex.Match(argValue);

            if (!match.Success)
            {
                throw new InvalidVersionStringException(argValue);
            }

            var versionMajorString = match.Groups[Regexes.DotNetCoreExtractionRegexMajorGroupName].Value;
            var versionMinorString = match.Groups[Regexes.DotNetCoreExtractionRegexMinorGroupName].Value;

            var versionMajor = int.Parse(versionMajorString);
            var versionMinor = int.Parse(versionMinorString);

            return sdks.Where(sdk => sdk.Version.Major == versionMajor && sdk.Version.Minor == versionMinor);
        }
    }
}
