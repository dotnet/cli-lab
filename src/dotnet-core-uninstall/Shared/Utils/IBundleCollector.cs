using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;

namespace Microsoft.DotNet.Tools.Uninstall.MacOs
{
    internal interface IBundleCollector
    {
        public IEnumerable<Bundle> GetAllInstalledBundles();

        public IEnumerable<BundleTypePrintInfo> GetSupportedBundleTypes();
    }
}
