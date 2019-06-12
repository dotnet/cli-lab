using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class MajorMinorOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<IBundleInfo> Filter(string argValue, IEnumerable<IBundleInfo> bundles)
        {
            var match = Regexes.BundleMajorMinorRegex.Match(argValue);

            if (!match.Success)
            {
                throw new InvalidVersionStringException(argValue);
            }

            var versionMajorString = match.Groups[Regexes.MajorGroupName].Value;
            var versionMinorString = match.Groups[Regexes.MinorGroupName].Value;

            var versionMajor = int.Parse(versionMajorString);
            var versionMinor = int.Parse(versionMinorString);

            return bundles.Where(bundle => bundle.Version.Major == versionMajor && bundle.Version.Minor == versionMinor);
        }
    }
}
