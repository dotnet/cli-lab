using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllBelowOptionFilterer : ArgFilterer<string>
    {
        public override bool AcceptMultipleBundleTypes { get; } = false;

        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(string argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var specifiedVersions = bundles
                .Select(bundle => bundle.Version)
                .Where(version => version.ToString().Equals(argValue))
                .OrderBy(version => version);

            if (specifiedVersions.Count() == 0)
            {
                throw new SpecifiedVersionNotFoundException(argValue);
            }

            var specifiedVersion = specifiedVersions.First();

            return bundles
                .Where(bundle => bundle.Version.CompareTo(specifiedVersion) < 0);
        }
    }
}
