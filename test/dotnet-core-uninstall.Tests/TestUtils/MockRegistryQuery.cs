using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.MacOs;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal class MockBundleCollector : IBundleCollector
    {
        private Dictionary<string, BundleArch> BundleSpecs = new Dictionary<string, BundleArch>
        {
            { "3.0.0", BundleArch.X64 },
            { "3.0.0-preview", BundleArch.X64 },
            { "1.0.1", BundleArch.X64 },
            { "1.0.0", BundleArch.X64 },
            { "3.0.1", BundleArch.X86 },
            { "3.0.2", BundleArch.X86 },
            { "3.0.2-preview1", BundleArch.X86 },
            { "3.0.2-preview2", BundleArch.X86 },
            { "3.1.0", BundleArch.X86 },
            { "2.1.1", BundleArch.X86 },
        };

        public IEnumerable<Bundle> GetAllInstalledBundles()
        {
            IEnumerable<Bundle> allBundles = new List<Bundle>();
            allBundles = allBundles.Concat(BundleSpecs.Select(pair => new Bundle<SdkVersion>(new SdkVersion(pair.Key), pair.Value, "--sdk", pair.Key)));
            allBundles = allBundles.Concat(BundleSpecs.Select(pair => new Bundle<RuntimeVersion>(new RuntimeVersion(pair.Key), pair.Value, "--runtime", pair.Key)));
            allBundles = allBundles.Concat(BundleSpecs.Select(pair => new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion(pair.Key), pair.Value, "--aspnet-runtime", pair.Key)));
            allBundles = allBundles.Concat(BundleSpecs.Select(pair => new Bundle<HostingBundleVersion>(new HostingBundleVersion(pair.Key), pair.Value, "--hosting-bundle", pair.Key)));
            return allBundles;
        }

        public IEnumerable<BundleTypePrintInfo> GetSupportedBundleTypes()
        {
            return Windows.SupportedBundleTypeConfigs.SupportedBundleTypes;
        }
    }
}
