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
        [InlineData(new string[] { "0.9.9" }, new bool[] { true })]
        [InlineData(new string[] { "0.0.0" }, new bool[] { false })]
        [InlineData(new string[] { "0.0.9" }, new bool[] { false })]
        [InlineData(new string[] { "0.9.9", "0.9.8" }, new bool[] { true, true })]
        [InlineData(new string[] { "0.1.0", "0.0.1" }, new bool[] { false, false })]
        [InlineData(new string[] { "0.9.9", "0.9.8", "0.0.0" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "0.9.9", "0.9.8", "0.0.9" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "0.0.0", "0.9.9", "0.0.1" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "0.0.0", "0.9.9", "0.1.0" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "0.0.0", "0.0.1", "0.0.9", "0.1.0"}, new bool[] { true, true, false, false })]
        internal void TestUninstallAllowed(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle<SdkVersion>>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, string.Empty));
            }
            bundles.ForEach(b => b.UninstallAllowed.Should().Be(true));

            var resultBundles = VSVersionHelper.AssignUninstallAllowed(bundles);

            for (int i = 0; i < versions.Length; i++)
            {
                resultBundles
                    .Where(b => b.Version is SdkVersion)
                    .ToArray()[i]
                    .UninstallAllowed.Should().Be(allowed[i]);
            }
        }

        [Theory]
        [InlineData(new string[] { "0.9.9" }, new bool[] { true })]
        [InlineData(new string[] { "0.0.0" }, new bool[] { false })]
        [InlineData(new string[] { "0.0.9" }, new bool[] { false })]
        [InlineData(new string[] { "0.9.9", "0.9.8" }, new bool[] { true, true })]
        [InlineData(new string[] { "0.1.0", "0.0.1" }, new bool[] { false, false })]
        [InlineData(new string[] { "0.9.9", "0.9.8", "0.0.0" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "0.9.9", "0.9.8", "0.0.9" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "0.0.0", "0.9.9", "0.0.1" }, new bool[] { true, true, false })]
        [InlineData(new string[] { "0.0.0", "0.9.9", "0.1.0" }, new bool[] { false, true, false })]
        [InlineData(new string[] { "0.0.0", "0.0.1", "0.0.9", "0.1.0" }, new bool[] { true, true, false, false })]
        internal void TestUninstallError(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }

            var resultBundles = VSVersionHelper.AssignUninstallAllowed(bundles);
            // Check we get an error when we're supposed to:
            if (allowed.Any(allowed => !allowed)) // If there are any sdk's that should be marked as uninstallable
            {
                var exception = Assert.Throws<UninstallationNotAllowedException>(() => VSVersionHelper.CheckUninstallable(resultBundles));
                for (int i = 0; i < versions.Length; i++) 
                {
                    if (!allowed[i])
                    {
                        Assert.Contains(versions[i], exception.Message);
                    } 
                    else
                    {
                        Assert.DoesNotContain(versions[i], exception.Message);
                    }
                }
            } 
            else
            {
                VSVersionHelper.CheckUninstallable(resultBundles); // Should not throw exception
            }
        }

        [Theory]
        [InlineData("0.0.0")]
        [InlineData("0.0.9")]
        internal void TestErrorWithMixedVersions(string version)
        {
            var resultBundles = VSVersionHelper.AssignUninstallAllowed(new List<Bundle>
            {
                new Bundle<SdkVersion>(new SdkVersion(version), new BundleArch(), string.Empty, version),
                new Bundle<RuntimeVersion>(new RuntimeVersion(), new BundleArch(), string.Empty, "RuntimeVersion"),
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion(), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion(), new BundleArch(), string.Empty, "HostingBundleVersion")
            });

            // Check that we still have all of the non-sdk bundles
            Assert.Contains(resultBundles, b => b.Version is RuntimeVersion);
            Assert.Contains(resultBundles, b => b.Version is HostingBundleVersion);
            Assert.Contains(resultBundles, b => b.Version is AspNetRuntimeVersion);

            // Check the we didn't mark any of the non-sdk's as uninstallable
            var exception = Assert.Throws<UninstallationNotAllowedException>(() => VSVersionHelper.CheckUninstallable(resultBundles));
            Assert.Contains(version, exception.Message);
            Assert.DoesNotContain("RuntimeVersion", exception.Message);
            Assert.DoesNotContain("AspNetVersion", exception.Message);
            Assert.DoesNotContain("HostingBundleVersion", exception.Message);
        }
    }
}
