using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Tests.Attributes;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Commands
{
    public class CommandBundleFilterTests
    {
        private static readonly string[] versions = { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200", "3.0.0", "3.0.1", "10.10.10" };

        [WindowsOnlyTheory]
        [InlineData("remove --all --sdk", new string[] { "1.0.0", "1.0.1", "3.0.0"})]
        [InlineData("dry-run --all --sdk", new string[] { "1.0.0", "1.0.1", "3.0.0" })]
        [InlineData("whatif --all --sdk", new string[] { "1.0.0", "1.0.1", "3.0.0" })]
        [InlineData("whatif --all-below 5.0.0 --sdk --force", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200", "3.0.0", "3.0.1" })]
        [InlineData("remove --all-below 5.0.0 --sdk --force", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200", "3.0.0", "3.0.1" })]
        [InlineData("remove --sdk 1.0.1", new string[] { "1.0.1" })]
        [InlineData("remove --sdk 1.0.0", new string[] { "1.0.0" })]
        [InlineData("remove --sdk 1.0.1 2.1.0 1.0.1", new string[] { "2.1.0", "1.0.1", "1.0.1" })]
        [InlineData("remove --sdk 1.0.0 1.0.1 1.1.0 2.1.0 2.1.500 2.1.600 2.2.100 2.2.200", 
            new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200" })]
        internal void TestRequiredUninstallableWhenExplicitlyAddedWindows(string command, string[] expectedUninstallable)
        {
            TestRequiredUninstallableWhenExplicitlyAdded(command, expectedUninstallable);
        }

        [MacOsOnlyTheory]
        [InlineData("remove --all-below 5.0.0 --sdk", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200", "3.0.0", "3.0.1" })] 
        [InlineData("dry-run --all-below 5.0.0 --sdk", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200", "3.0.0", "3.0.1" })]
        [InlineData("whatif --all-below 5.0.0 --sdk", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200", "3.0.0", "3.0.1" })]
        internal void TestRequiredUninstallableWhenExplicitlyAddedMac(string command, string[] expectedUninstallable)
        {
            TestRequiredUninstallableWhenExplicitlyAdded(command, expectedUninstallable);
        }

        internal void TestRequiredUninstallableWhenExplicitlyAdded(string command, string[] expectedUninstallable)
        {
            var bundles = new List<Bundle<SdkVersion>>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }

            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);
            var uninstallableBundles = CommandBundleFilter.GetFilteredBundles(bundles, parseResult)
                .Select(b => b.DisplayName);
            var requiredBundles = versions.Where(v => !uninstallableBundles.Contains(v));

            (uninstallableBundles.Count() + requiredBundles.Count()).Should().Be(versions.Length);
            uninstallableBundles.ToHashSet().Should().BeEquivalentTo(expectedUninstallable.ToHashSet());
            requiredBundles.Should().BeEquivalentTo(versions.Where(v => !expectedUninstallable.Contains(v)));
        }

        [WindowsOnlyTheory]
        [InlineData("remove --sdk 10.10.10")]
        [InlineData("remove --sdk --all --force")]
        [InlineData("remove --sdk 1.0.0 1.0.1 1.1.0 2.1.0 2.1.500 2.1.600 2.2.100 2.2.200 3.0.0 3.0.1 10.10.10")]
        internal void TestUpperLimitAlwaysRequiredWindows(string command)
        {
            var bundles = new List<Bundle<SdkVersion>>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }

            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);
            Action filteringAction = () => CommandBundleFilter.GetFilteredBundles(bundles, parseResult);
            filteringAction.Should().Throw<UninstallationNotAllowedException>();
        }
    }
}
