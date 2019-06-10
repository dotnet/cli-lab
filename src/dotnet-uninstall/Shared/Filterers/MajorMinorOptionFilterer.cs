using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;
using static Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class MajorMinorOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<ISdkInfo> Filter(string argValue, IEnumerable<ISdkInfo> sdks)
        {
            var regex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)$");
            var match = regex.Match(argValue);

            if (!match.Success)
            {
                throw new InvalidVersionStringException(argValue);
            }

            var versionMajorString = match.Groups["major"].Value;
            var versionMinorString = match.Groups["minor"].Value;

            var versionMajor = int.Parse(versionMajorString);
            var versionMinor = int.Parse(versionMinorString);

            return sdks.Where(sdk => sdk.Version.Major == versionMajor && sdk.Version.Minor == versionMinor);
        }
    }
}
