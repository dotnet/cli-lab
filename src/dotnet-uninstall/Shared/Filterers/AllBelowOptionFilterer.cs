using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class AllBelowOptionFilterer : ArgFilterer<string>
    {
        public override bool AcceptMultipleBundleTypes { get; } = false;

        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(string argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            try
            {
                var version = BundleVersion.FromInput<TBundleVersion>(argValue) as TBundleVersion;

                return bundles
                    .Where(bundle => bundle.Version.CompareTo(version) < 0);
            }
            catch (InvalidInputVersionException)
            {
                var majorMinor = new MajorMinorVersion(argValue);

                return bundles
                    .Where(bundle => bundle.Version.MajorMinor.CompareTo(majorMinor) < 0);
            }
        }
    }
}
