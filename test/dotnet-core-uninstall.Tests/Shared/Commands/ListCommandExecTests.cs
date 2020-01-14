using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Tests.Attributes;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Commands
{
    public class ListCommandExecTests
    {
        private Dictionary<string, BundleArch> versionsWithArch = new Dictionary<string, BundleArch>
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

        [WindowsOnlyTheory]
        [InlineData("sdk", "", new string[] { "1.0.1", "3.1.0", "2.1.1" }, new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("runtime", "", new string[] { }, new string[] { "1.0.1", "3.1.0", "2.1.1", "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2" })]
        [InlineData("sdk", "--x64", new string[] { "1.0.1" }, new string[] { "3.0.0", "3.0.0-preview", "1.0.0" })]
        internal void ListCommandFilteringIsCorrectOnWindows(string bundleType, string options, string[] expectedProtected, string[] expectedUninstallable)
        {
            ListCommandFilteringIsCorrect(bundleType, options, expectedProtected, expectedUninstallable);
        }

        [MacOsOnlyTheory]
        [InlineData("sdk", "", new string[] { "3.1.0" }, new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2", "2.1.1", "1.0.1" })]
        [InlineData("runtime", "", new string[] { "3.1.0", "3.0.2", "2.1.1", "1.0.1" }, new string[] { "3.0.0", "3.0.0-preview", "1.0.0", "3.0.1", "3.0.2-preview1", "3.0.2-preview2" })]
        internal void ListCommandFilteringIsCorrectOnMac(string bundleType, string options, string[] expectedProtected, string[] expectedUninstallable)
        {
            ListCommandFilteringIsCorrect(bundleType, options, expectedProtected, expectedUninstallable);
        }

        private void ListCommandFilteringIsCorrect(string bundleType, string options, string[] expectedProtected, string[] expectedUninstallable)
        {
            var bundles = new List<Bundle>();
            foreach (var pair in versionsWithArch)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(pair.Key), pair.Value, "sdk", pair.Key));
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(pair.Key), pair.Value, "runtime", pair.Key));
            }
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"list --{ bundleType } { options }");
            var result = ListCommandExec.GetFilteredBundlesWithRequirements(bundles, SupportedBundleTypeConfigs.GetSupportedBundleTypes(), parseResult);

            result.Values.Count().Should().Be(1);
            var bundleResults = result.FirstOrDefault();
            bundleResults.Key.Type.Should().Be(bundleType.Equals("sdk") ? BundleType.Sdk : BundleType.Runtime);
            bundleResults.Should().NotBeNull();

            bundleResults.Value.Where(pair => pair.Value.Equals(string.Empty)).Select(pair => pair.Key.DisplayName)
                .Should().BeEquivalentTo(expectedUninstallable);
            bundleResults.Value.Where(pair => !pair.Value.Equals(string.Empty)).Select(pair => pair.Key.DisplayName)
                .Should().BeEquivalentTo(expectedProtected);
        }
    }
}
