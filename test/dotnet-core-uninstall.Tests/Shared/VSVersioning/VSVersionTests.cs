using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
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
        internal void TestUninstallAllowed(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle<SdkVersion>>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, string.Empty));
            }
            bundles.ForEach(b => b.UninstallAllowed.Should().Be(true));

            VisualStudioSafeVersionsExtracter.AssignUninstallAllowed(bundles);

            for (int i = 0; i < versions.Length; i++)
            {
                bundles
                    .Where(b => b.Version is SdkVersion)
                    .ToArray()[i]
                    .UninstallAllowed.Should().Be(allowed[i]);
            }
        }

        [Theory]
        [InlineData("1.0.0")]
        [InlineData("2.1.0", "1.0.1" )]
        [InlineData("2.1.500", "2.1.600")]
        [InlineData("2.2.100", "2.2.200" )]
        internal void TestUninstallError(params string[] versions)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }

            VisualStudioSafeVersionsExtracter.AssignUninstallAllowed(bundles);
            // None of the bundles are uninstallable-> throw error
            var exception = Assert.Throws<UninstallationNotAllowedException>(() => VisualStudioSafeVersionsExtracter.RemoveUninstallableBundles(bundles));
            foreach (string v in versions) 
            {
                Assert.Contains(v, exception.Message);
            }
        }

        [Theory]
        [InlineData(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.1.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "2.0.0" }, new bool[] { true, false, false })]
        [InlineData(new string[] { "1.0.0", "1.0.1", "1.0.2" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "2.2.100", "2.2.200", "2.2.300" }, new bool[] { false, true, false })]
        internal void TestRemoveNotUninstallable(string[] versions, bool[] allowed)
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

            VisualStudioSafeVersionsExtracter.AssignUninstallAllowed(bundles);

            // Check that we still have all of the non-sdk bundles
            Assert.Contains(bundles, b => b.Version is RuntimeVersion);
            Assert.Contains(bundles, b => b.Version is HostingBundleVersion);
            Assert.Contains(bundles, b => b.Version is AspNetRuntimeVersion);

            // Check the we didn't mark any of the non-sdk's as uninstallable
            var lst = VisualStudioSafeVersionsExtracter.RemoveUninstallableBundles(bundles).Select(i => i.DisplayName);
            Assert.Contains("RuntimeVersion", lst);
            Assert.Contains("AspNetVersion", lst);
            Assert.Contains("HostingBundleVersion", lst);
            for (int i = 0; i < versions.Length; i++)
            {
                if (allowed[i])
                {
                    Assert.Contains(versions[i], lst);
                } 
                else
                {
                    Assert.DoesNotContain(versions[i], lst);
                }
            }
        }
    }
}
