using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllBelowOptionFilterer : ArgFilterer<string>
    {
        public override IEnumerable<Bundle> Filter(string argValue, IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            if ((int)typeSelection < 1 || typeSelection > (BundleType.Sdk | BundleType.Runtime))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (typeSelection != BundleType.Sdk || typeSelection != BundleType.Runtime)
            {
                throw new BundleTypeNotSpecifiedException();
            }

            var bundlesWithSelectedType = bundles
                .Where(bundle => (typeSelection & bundle.Version.Type) > 0);

            var specifiedVersions = bundlesWithSelectedType
                .Select(bundle => bundle.Version)
                .Where(version => version.ToString().Equals(argValue))
                .OrderBy(version => version);

            if (specifiedVersions.Count() == 0)
            {
                throw new SpecifiedVersionNotFoundException(argValue);
            }

            var specifiedVersion = specifiedVersions.First();

            switch (typeSelection)
            {
                case BundleType.Sdk:
                    return bundlesWithSelectedType
                        .Where(bundle => (bundle.Version as SdkVersion).CompareTo(specifiedVersion) < 0);
                case BundleType.Runtime:
                    return bundlesWithSelectedType
                        .Where(bundle => (bundle.Version as RuntimeVersion).CompareTo(specifiedVersion) < 0);
                default:
                    throw new BundleTypeNotSpecifiedException();
            }
        }
    }
}
