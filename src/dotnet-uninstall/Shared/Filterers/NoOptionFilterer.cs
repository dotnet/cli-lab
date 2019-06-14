using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class NoOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override bool AcceptMultipleBundleTypes { get; } = false;

        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<string> argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var anyMissing = argValue
                .Where(next => !bundles
                    .Select(bundle => bundle.Version.ToString())
                    .ToList()
                    .Contains(next));

            if (anyMissing.Count() > 0)
            {
                throw new SpecifiedVersionNotFoundException(anyMissing.First());
            }

            var specifiedVersions = bundles
                .Select(bundle => bundle.Version)
                .Where(version => argValue.Contains(version.ToString()))
                .OrderBy(version => version);

            return bundles
                .Where(bundle => specifiedVersions.Contains(bundle.Version));
        }
    }
}
