using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;
using Microsoft.DotNet.Tools.Uninstall.Tests.Attributes;
using NuGet.Versioning;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.VSVersioning
{
    public class VSVersionTests
    {
        [WindowsOnlyTheory]
        [InlineData(new string[] { }, new bool[] { })]
        [InlineData(new string[] { "1.0.0" }, new bool[] { false })]
        [InlineData(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [InlineData(new string[] { "2.1.0", "1.0.1" }, new bool[] { false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.1.0" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "2.0.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.0.2" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "2.1.500", "2.1.600" }, new bool[] { false, false })]
        [InlineData(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "2.2.100", "2.2.200" }, new bool[] { false, false })]
        [InlineData(new string[] { "2.2.100", "2.2.200", "2.2.300" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "3.0.0", "3.0.1", "5.0.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "5.0.0", "5.0.1", "10.100.100" }, new bool[] { false, false, false })]
        internal void TestGetUninstallableWindows(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, string.Empty));
            }

            var uninstallable = VisualStudioSafeVersionsExtractor.GetUninstallableBundles(bundles);

            CheckAllowed(bundles, uninstallable, allowed, null);
        }

        [MacOsOnlyTheory]
        [InlineData(new string[] { }, new bool[] { }, new string[] { }, new bool[] { })]
        [InlineData(new string[] { "1.0.0" }, new bool[] { false }, new string[] { }, new bool[] { })]
        [InlineData(new string[] { }, new bool[] { }, new string[] { "1.0.0" }, new bool[] { false })]
        [InlineData(new string[] { "1.0.0" }, new bool[] { false }, new string[] { "1.0.0" }, new bool[] { false })]
        [InlineData(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false }, new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [InlineData(new string[] { "2.1.0", "1.0.1" }, new bool[] { false, true }, new string[] { "1.0.0", "1.1.0" }, new bool[] { false, false })]
        [InlineData(new string[] { "3.0.0", "5.0.0" }, new bool[] { false, false }, new string[] { "1.0.0", "1.1.0", "1.0.1", "1.0.2", "1.1.3" }, new bool[] { true, true, true, false, false })]
        [InlineData(new string[] { "3.0.0", "5.0.0" }, new bool[] { false, false }, new string[] { "1.0.0", "1.1.0", "1.0.1", "5.0.0" }, new bool[] { true, false, false, false })]
        [InlineData(new string[] { "5.0.0", "5.0.1", "10.100.100" }, new bool[] { false, false, false }, new string[] { "5.0.0", "10.0.0" }, new bool[] { false, false })]
        internal void TestGetUninstallableMac(string[] sdkVersions, bool[] sdkAllowed, string[] runtimeVersions,  bool[] runtimeAllowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in sdkVersions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, string.Empty));
            }
            foreach (string v in runtimeVersions)
            {
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), string.Empty, string.Empty));
            }

            var uninstallable = VisualStudioSafeVersionsExtractor.GetUninstallableBundles(bundles);

            CheckAllowed(bundles, uninstallable, sdkAllowed, runtimeAllowed);
        }

        [WindowsOnlyTheory]
        [InlineData(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.1.0" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "2.0.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.0.2" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "2.2.100", "2.2.200", "2.2.300" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "5.0.0", "5.0.1", "10.100.100" }, new bool[] { false, false, false })]
        internal void TestGetUninstallableNonSdkVersionsWindows(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }
            TestGetUninstallableNonSdkVersions(bundles, allowed, null);
        }

        [MacOsOnlyTheory]
        [InlineData(new string[] { "1.0.0" }, new bool[] { false }, new string[] { "1.0.0" }, new bool[] { false })]
        [InlineData(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false }, new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [InlineData(new string[] { "2.1.0", "1.0.1" }, new bool[] { false, true }, new string[] { "2.0.0", "1.1.0" }, new bool[] { false, false })]
        [InlineData(new string[] { "3.0.0", "5.0.0" }, new bool[] { false, false }, new string[] { "1.0.0", "1.1.0", "1.0.1", "1.0.2", "1.1.3" }, new bool[] { true, true, true, false, false })]
        [InlineData(new string[] { "3.0.0", "5.0.0" }, new bool[] { false, false }, new string[] { "1.0.0", "1.1.0", "1.0.1", "5.0.0" }, new bool[] { true, false, false, false })]
        [InlineData(new string[] { "5.0.0", "5.0.1", "10.100.100" }, new bool[] { false, false, false }, new string[] { "5.0.0", "10.0.0" }, new bool[] { false, false })]
        internal void TestGetUninstallableNonSdkVersionsMac(string[] sdkVersions, bool[] sdkAllowed, string[] runtimeVersions, bool[] runtimeAllowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in sdkVersions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }
            foreach (string v in runtimeVersions)
            {
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), string.Empty, v));
            }
            TestGetUninstallableNonSdkVersions(bundles, sdkAllowed, runtimeAllowed);
        }

        internal void TestGetUninstallableNonSdkVersions(IEnumerable<Bundle> bundles, bool[] sdkAllowed, bool[] runtimeAllowed)
        {
            bundles = bundles.Concat(new List<Bundle>
            { 
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion("1.0.0"), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion("10.0.0"), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion("1.0.0"), new BundleArch(), string.Empty, "HostingBundleVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion("10.0.0"), new BundleArch(), string.Empty, "HostingBundleVersion")
            });

            var uninstallable = VisualStudioSafeVersionsExtractor.GetUninstallableBundles(bundles);

            // Check that we still have all of the non-sdk bundles
            uninstallable.Where(b => b.Version is AspNetRuntimeVersion).Should().HaveCount(1);
            uninstallable.Where(b => b.Version is HostingBundleVersion).Should().HaveCount(1);

            CheckAllowed(bundles, uninstallable, sdkAllowed, runtimeAllowed);
        }

        private void CheckAllowed(IEnumerable<Bundle> bundles, IEnumerable<Bundle> uninstallable, bool[] sdkAllowed, bool[] runtimeAllowed)
        {
            var sdkBundles = bundles.Where(bundle => bundle.Version is SdkVersion).ToArray();
            var runtimeBundles = bundles.Where(bundle => bundle.Version is RuntimeVersion).ToArray();
            var otherBundles = bundles.Except(sdkBundles).Except(runtimeBundles);
            for (int i = 0; i < sdkBundles.Count(); i++)
            {
                if (sdkAllowed[i])
                {
                    uninstallable.Should().Contain(sdkBundles[i]);
                }
                else
                {
                    uninstallable.Should().NotContain(sdkBundles[i]);
                }
            }
            
            for (int i = 0; i < runtimeBundles.Count(); i++)
            {
                if (runtimeAllowed[i])
                {
                    uninstallable.Should().Contain(runtimeBundles[i]);
                }
                else
                {
                    uninstallable.Should().NotContain(runtimeBundles[i]);
                }
            }
            // Check others are uninstallable unless their version is above the upper limit
            foreach (Bundle bundle in otherBundles)
            {
                if (bundle.Version.SemVer > VisualStudioSafeVersionsExtractor.UpperLimit)
                {
                    uninstallable.Should().NotContain(bundle);
                }
                else
                {
                    uninstallable.Should().Contain(bundle);
                }
            }
        }

        [WindowsOnlyTheory]
        [InlineData(new string[] { }, new string[] { })]
        [InlineData(new string[] { "1.0.1", "1.0.0" }, new string[] { "", "None" })]
        [InlineData(new string[] { "2.3.0", "2.1.800", "2.1.300" }, new string[] { "", " 2019", " 2017" })]
        [InlineData(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new string[] { " 2017", "None", " 2019" })]
        [InlineData(new string[] { "2.1.500", "5.0.1", "5.0.0" }, new string[] { " 2017", "5.0.0", "5.0.0" })]
        internal void TestGetListCommandUninstallableStringsWindows(string[] versions, string[] expectedStrings)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }

            TestGetListCommandUninstallableStrings(bundles, ConvertStringInput(expectedStrings), new string[0]);
        }

        [MacOsOnlyTheory]
        [InlineData(new string[] { }, new string[] { }, new string[] { }, new string[] { })]
        [InlineData(new string[] { }, new string[] { }, new string[] { "1.0.0" }, new string[] { " or SDKs" })]
        [InlineData(new string[] { "1.0.0" }, new string[] { "" }, new string[] { }, new string[] { })]
        [InlineData(new string[] { "1.0.0" }, new string[] { "" }, new string[] { "1.0.0" }, new string[] { " or SDKs" })]
        [InlineData(new string[] { "1.0.0", "1.0.1" }, new string[] { "None", "" }, new string[] { "1.0.0", "1.0.1" }, new string[] { "None", " or SDKs" })]
        [InlineData(new string[] { "2.1.0", "1.0.1" }, new string[] { "", "None" }, new string[] { "2.0.0", "1.1.0" }, new string[] { " or SDKs", " or SDKs" })]
        [InlineData(new string[] { "3.0.0", "5.0.0" }, new string[] { "", "5.0.0" }, new string[] { "1.0.0", "1.1.0", "1.0.1", "1.0.2", "1.1.3" }, new string[] { "None", "None", "None", " or SDKs", " or SDKs" })]
        [InlineData(new string[] { "3.0.0", "5.0.0" }, new string[] { "", "5.0.0" }, new string[] { "1.0.0", "1.1.0", "1.0.1", "5.0.0" }, new string[] { "None", " or SDKs", " or SDKs", "5.0.0" })]
        [InlineData(new string[] { "5.0.0", "5.0.1", "10.100.100" }, new string[] { "5.0.0", "5.0.0", "5.0.0" }, new string[] { "5.0.0", "10.0.0" }, new string[] { "5.0.0", "5.0.0" })]
        internal void TestGetListCommandUninstallableStringsMac(string[] sdkVersions, string[] sdkExpected, string[] runtimeVersions, string[] runtimeExpected)
        {
            var bundles = new List<Bundle>();
            foreach (string v in sdkVersions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }
            foreach (string v in runtimeVersions)
            {
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), string.Empty, v));
            }

            TestGetListCommandUninstallableStrings(bundles, ConvertStringInput(sdkExpected), ConvertStringInput(runtimeExpected));
        }

        internal void TestGetListCommandUninstallableStrings(IEnumerable<Bundle> bundles, string[] sdkExpected, string[] runtimeExpected)
        {
            bundles = bundles.Concat(new List<Bundle>
            {
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion("1.0.0"), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion("10.0.0"), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion("1.0.0"), new BundleArch(), string.Empty, "HostingBundleVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion("10.0.0"), new BundleArch(), string.Empty, "HostingBundleVersion")
            });

            var strings = VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(bundles);
            strings.Count().Should().Be(bundles.Count());

            var sdkBundles = strings.Where(pair => pair.Key.Version is SdkVersion).ToArray();
            var sdkStrings = sdkBundles.Select(pair => pair.Value);
            var runtimeBundles = strings.Where(pair => pair.Key.Version is RuntimeVersion).ToArray();
            var runtimeStrings = runtimeBundles.Select(pair => pair.Value);
            var otherBundles = strings.Except(sdkBundles).Except(runtimeBundles);

            sdkStrings.Should().BeEquivalentTo(sdkExpected);
            runtimeStrings.Should().BeEquivalentTo(runtimeExpected);

            otherBundles.Should().HaveCount(4);
            // All bundles above the upper limit are required
            otherBundles.Where(pair => pair.Key.Version.SemVer >= VisualStudioSafeVersionsExtractor.UpperLimit)
                .ToList().ForEach(str => str.Value.Should().Be(string.Format(LocalizableStrings.UpperLimitRequirement, VisualStudioSafeVersionsExtractor.UpperLimit)));
            // Non-sdk bundles are always uninstallable below the upper limit
            otherBundles.Where(pair => pair.Key.Version.SemVer < VisualStudioSafeVersionsExtractor.UpperLimit)
                .ToList().ForEach(str => str.Value.Should().Be(string.Empty));
        }

        private string[] ConvertStringInput(string[] input)
        {
            var output = new string[input.Length];
            
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i].Equals("None") ? string.Empty :
                    SemanticVersion.TryParse(input[i], out _) ? string.Format(LocalizableStrings.UpperLimitRequirement, VisualStudioSafeVersionsExtractor.UpperLimit) :
                    string.Format(LocalizableStrings.RequirementExplainationString, RuntimeInfo.RunningOnOSX ? " for Mac" + input[i] : input[i]);
            }

            return output;
        }
    }
}
