using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.VSVersioning
{
    public class VSVersionTests
    {
        [Theory]
        [InlineData(new string[] { "1.0.0" }, new bool[] { false })]
        [InlineData(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [InlineData(new string[] { "2.1.0", "1.0.1" }, new bool[] { false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.1.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "2.0.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.0.2" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "2.1.500", "2.1.600" }, new bool[] { false, false })]
        [InlineData(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "2.2.100", "2.2.200" }, new bool[] { false, false })]
        [InlineData(new string[] { "2.2.100", "2.2.200", "2.2.300" }, new bool[] { false, true, false })]
        internal void TestGetUninstallable(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle<SdkVersion>>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, string.Empty));
            }

            var uninstallable = VisualStudioSafeVersionsExtractor.GetUninstallableBundles(bundles);

            CheckAllowed(bundles, uninstallable, allowed);
        }

        [Theory]
        [InlineData(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.1.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "2.0.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.0.2" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "2.2.100", "2.2.200", "2.2.300" }, new bool[] { false, true, false })]
        internal void TestGetUninstallableNonSdkVersions(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle>
            {
                new Bundle<RuntimeVersion>(new RuntimeVersion(), new BundleArch(), string.Empty, "RuntimeVersion"),
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion(), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion(), new BundleArch(), string.Empty, "HostingBundleVersion")
            };
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }

            var uninstallable = VisualStudioSafeVersionsExtractor.GetUninstallableBundles(bundles);

            // Check that we still have all of the non-sdk bundles
            bundles.Should().Contain(b => b.Version is RuntimeVersion);
            bundles.Should().Contain(b => b.Version is AspNetRuntimeVersion);
            bundles.Should().Contain(b => b.Version is HostingBundleVersion);

            CheckAllowed(bundles, uninstallable, allowed);
        }

        private void CheckAllowed(IEnumerable<Bundle> bundles, IEnumerable<Bundle> uninstallable, bool[] allowed)
        {
            var fullList = bundles.Where(b => b.Version is SdkVersion).ToArray();
            for (int i = 0; i < fullList.Count(); i++)
            {
                if (allowed[i])
                {
                    uninstallable.Should().Contain(fullList[i]);
                }
                else
                {
                    uninstallable.Should().NotContain(fullList[i]);
                }
            }
        }
    }
}
