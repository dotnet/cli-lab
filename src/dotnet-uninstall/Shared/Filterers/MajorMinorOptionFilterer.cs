using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class MajorMinorOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<Bundle> Filter(string argValue, IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            if ((int)typeSelection < 1 || typeSelection > (BundleType.Sdk | BundleType.Runtime))
            {
                throw new ArgumentOutOfRangeException();
            }

            var match = Regexes.BundleMajorMinorRegex.Match(argValue);

            if (!match.Success)
            {
                throw new InvalidInputVersionStringException(argValue);
            }

            var versionMajorString = match.Groups[Regexes.MajorGroupName].Value;
            var versionMinorString = match.Groups[Regexes.MinorGroupName].Value;

            var versionMajor = int.Parse(versionMajorString);
            var versionMinor = int.Parse(versionMinorString);

            var sdkBundles = ((typeSelection & BundleType.Sdk) > 0) ?
                bundles.Where(bundle => bundle.Version is SdkVersion && bundle.Version.Major == versionMajor && bundle.Version.Minor == versionMinor) :
                new List<Bundle>();

            var runtimeBundles = ((typeSelection & BundleType.Runtime) > 0) ?
                bundles.Where(bundle => bundle.Version is RuntimeVersion && bundle.Version.Major == versionMajor && bundle.Version.Minor == versionMinor) :
                new List<Bundle>();

            return sdkBundles.Concat(runtimeBundles);
        }
    }
}
