using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Filterers;
using Microsoft.DotNet.Tools.Uninstall.Tests.Attributes;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public abstract class FiltererTests
    {
        internal abstract Option Option { get; }
        internal abstract string DefaultTestArgValue { get; }
        internal virtual Filterer OptionFilterer => OptionFilterers.OptionFiltererDictionary[Option.Name];

        internal const BundleArch DefaultTestArchSelection = BundleArch.X86 | BundleArch.X64;

        private static readonly string TestUninstallCommand = "some random uninstall command";
        private static readonly string TestDisplayName = "some random display name";

        internal static Bundle<SdkVersion> Sdk_3_0_100_P5_X64 = GetBundleFromInput<SdkVersion>("3.0.100-preview5-011568", BundleArch.X64);
        internal static Bundle<SdkVersion> Sdk_2_2_300_X64 = GetBundleFromInput<SdkVersion>("2.2.300", BundleArch.X64);
        internal static Bundle<SdkVersion> Sdk_2_2_222_X86 = GetBundleFromInput<SdkVersion>("2.2.222", BundleArch.X86);
        internal static Bundle<SdkVersion> Sdk_2_2_202_X64 = GetBundleFromInput<SdkVersion>("2.2.202", BundleArch.X64);
        internal static Bundle<SdkVersion> Sdk_2_2_202_X86 = GetBundleFromInput<SdkVersion>("2.2.202", BundleArch.X86);
        internal static Bundle<SdkVersion> Sdk_2_1_700_X64 = GetBundleFromInput<SdkVersion>("2.1.700", BundleArch.X64);
        internal static Bundle<SdkVersion> Sdk_2_1_300_Rc1_X64 = GetBundleFromInput<SdkVersion>("2.1.300-rc1-008673", BundleArch.X64);
        internal static Bundle<SdkVersion> Sdk_2_1_300_Rc1_X86 = GetBundleFromInput<SdkVersion>("2.1.300-rc1-008673", BundleArch.X86);

        internal static Bundle<RuntimeVersion> Runtime_3_0_0_P5_X64 = GetBundleFromInput<RuntimeVersion>("3.0.0-preview5-27626-15", BundleArch.X64);
        internal static Bundle<RuntimeVersion> Runtime_3_0_0_P5_X86 = GetBundleFromInput<RuntimeVersion>("3.0.0-preview5-27626-15", BundleArch.X86);
        internal static Bundle<RuntimeVersion> Runtime_3_0_0_P_X86 = GetBundleFromInput<RuntimeVersion>("3.0.0-preview-27122-01", BundleArch.X86);
        internal static Bundle<RuntimeVersion> Runtime_2_2_5_X64 = GetBundleFromInput<RuntimeVersion>("2.2.5", BundleArch.X64);
        internal static Bundle<RuntimeVersion> Runtime_2_2_5_X86 = GetBundleFromInput<RuntimeVersion>("2.2.5", BundleArch.X86);
        internal static Bundle<RuntimeVersion> Runtime_2_2_4_X86 = GetBundleFromInput<RuntimeVersion>("2.2.4", BundleArch.X86);
        internal static Bundle<RuntimeVersion> Runtime_2_2_2_X64 = GetBundleFromInput<RuntimeVersion>("2.2.2", BundleArch.X64);
        internal static Bundle<RuntimeVersion> Runtime_2_1_0_Rc1_X64 = GetBundleFromInput<RuntimeVersion>("2.1.0-rc1", BundleArch.X64);

        internal static Bundle<AspNetRuntimeVersion> AspNetRuntime_3_0_0_P6_X64 = GetBundleFromInput<AspNetRuntimeVersion>("3.0.0-preview6.19307.2", BundleArch.X64);
        internal static Bundle<AspNetRuntimeVersion> AspNetRuntime_3_0_0_P_X64 = GetBundleFromInput<AspNetRuntimeVersion>("3.0.0-preview-18579-0056", BundleArch.X64);
        internal static Bundle<AspNetRuntimeVersion> AspNetRuntime_2_2_6_X64 = GetBundleFromInput<AspNetRuntimeVersion>("2.2.6", BundleArch.X64);
        internal static Bundle<AspNetRuntimeVersion> AspNetRuntime_2_2_3_X64 = GetBundleFromInput<AspNetRuntimeVersion>("2.2.3", BundleArch.X64);
        internal static Bundle<AspNetRuntimeVersion> AspNetRuntime_2_1_0_Rc1_X64 = GetBundleFromInput<AspNetRuntimeVersion>("2.1.0-rc1-final", BundleArch.X64);

        internal static Bundle<HostingBundleVersion> HostingBundle_3_0_0_P6_X86 = GetBundleFromInput<HostingBundleVersion>("3.0.0-preview6.19307.2", BundleArch.X86);
        internal static Bundle<HostingBundleVersion> HostingBundle_3_0_0_P4_X86 = GetBundleFromInput<HostingBundleVersion>("3.0.0-preview4-19216-03", BundleArch.X86);
        internal static Bundle<HostingBundleVersion> HostingBundle_2_2_6_X86 = GetBundleFromInput<HostingBundleVersion>("2.2.6", BundleArch.X86);
        internal static Bundle<HostingBundleVersion> HostingBundle_2_2_0_X86 = GetBundleFromInput<HostingBundleVersion>("2.2.0", BundleArch.X86);
        internal static Bundle<HostingBundleVersion> HostingBundle_2_2_0_P3_X86 = GetBundleFromInput<HostingBundleVersion>("2.2.0-preview3-35497", BundleArch.X86);
        internal static Bundle<HostingBundleVersion> HostingBundle_2_2_0_P1_X86 = GetBundleFromInput<HostingBundleVersion>("2.2.0-preview1-35029", BundleArch.X86);
        internal static Bundle<HostingBundleVersion> HostingBundle_2_1_0_Rc1_X86 = GetBundleFromInput<HostingBundleVersion>("2.1.0-rc1-final", BundleArch.X86);
        internal static Bundle<HostingBundleVersion> HostingBundle_1_1_13_X86 = GetBundleFromInput<HostingBundleVersion>("1.1.13", BundleArch.X86);

        internal static readonly IEnumerable<Bundle<SdkVersion>> DefaultTestSdks = new List<Bundle<SdkVersion>>
        {
            Sdk_2_2_202_X64,
            Sdk_2_1_300_Rc1_X64,
            Sdk_2_2_222_X86,
            Sdk_2_2_202_X86,
            Sdk_2_1_700_X64,
            Sdk_3_0_100_P5_X64,
            Sdk_2_2_300_X64,
            Sdk_2_1_300_Rc1_X86
        };

        internal static readonly IEnumerable<Bundle<RuntimeVersion>> DefaultTestRuntimes = new List<Bundle<RuntimeVersion>>
        {
            Runtime_2_2_2_X64,
            Runtime_3_0_0_P5_X64,
            Runtime_2_2_4_X86,
            Runtime_2_2_5_X86,
            Runtime_3_0_0_P5_X86,
            Runtime_2_2_5_X64,
            Runtime_3_0_0_P_X86,
            Runtime_2_1_0_Rc1_X64
        };

        internal static readonly IEnumerable<Bundle<AspNetRuntimeVersion>> DefaultTestAspNetRuntimes = new List<Bundle<AspNetRuntimeVersion>>
        {
            AspNetRuntime_3_0_0_P6_X64,
            AspNetRuntime_3_0_0_P_X64,
            AspNetRuntime_2_2_6_X64,
            AspNetRuntime_2_2_3_X64,
            AspNetRuntime_2_1_0_Rc1_X64
        };

        internal static readonly IEnumerable<Bundle<HostingBundleVersion>> DefaultTestHostingBundles = new List<Bundle<HostingBundleVersion>>
        {
            HostingBundle_3_0_0_P6_X86,
            HostingBundle_3_0_0_P4_X86,
            HostingBundle_2_2_6_X86,
            HostingBundle_2_2_0_X86,
            HostingBundle_2_2_0_P3_X86,
            HostingBundle_2_2_0_P1_X86,
            HostingBundle_2_1_0_Rc1_X86,
            HostingBundle_1_1_13_X86
        };

        internal static readonly IEnumerable<Bundle> DefaultTestBundles = DefaultTestSdks
            .Select(sdk => sdk as Bundle)
            .Concat(DefaultTestRuntimes.Select(runtime => runtime as Bundle))
            .Concat(DefaultTestAspNetRuntimes.Select(aspNetRuntime => aspNetRuntime as Bundle))
            .Concat(DefaultTestHostingBundles.Select(hostingBundle => hostingBundle as Bundle));

        [Theory]
        [InlineData((BundleType)0)]
        [InlineData((BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle) + 1)]
        [InlineData((BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle) + 2)]
        [InlineData((BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle) + 10)]
        internal void TestFiltererArgumentOutOfRangeException(BundleType typeSelection)
        {
            TestFiltererException<ArgumentOutOfRangeException>(DefaultTestBundles, DefaultTestArgValue, typeSelection, DefaultTestArchSelection);
        }

        [WindowsOnlyFact]
        internal void TestFiltererBundleTypeNotSpecifiedExceptionWindows()
        {
            TestFiltererException<BundleTypeMissingException>(
                DefaultTestBundles,
                DefaultTestArgValue,
                BundleType.Sdk | BundleType.Runtime,
                DefaultTestArchSelection,
                string.Format(LocalizableStrings.BundleTypeMissingExceptionMessage, "--aspnet-runtime, --hosting-bundle, --runtime, --sdk"));
        }

        [MacOsOnlyFact]
        internal void TestFiltererBundleTypeNotSpecifiedExceptionMacOs()
        {
            TestFiltererException<BundleTypeMissingException>(
                DefaultTestBundles,
                DefaultTestArgValue,
                BundleType.Sdk | BundleType.Runtime,
                DefaultTestArchSelection,
                string.Format(LocalizableStrings.BundleTypeMissingExceptionMessage, "--runtime, --sdk"));
        }

        internal virtual void TestFiltererGood(IEnumerable<Bundle> testBundles, string argValue, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"dry-run --{Option.Name} {argValue}");

            OptionFilterer.Filter(parseResult, Option, testBundles, typeSelection, archSelection)
                .Should().BeEquivalentTo(expected);
        }

        internal virtual void TestFiltererException<TException>(IEnumerable<Bundle> testBundles, string argValue, BundleType typeSelection, BundleArch archSelection)
            where TException : Exception
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"dry-run --{Option.Name} {argValue}");
            Action action = () => OptionFilterer.Filter(parseResult, Option, testBundles, typeSelection, archSelection);

            action.Should().Throw<TException>();
        }

        internal virtual void TestFiltererException<TException>(IEnumerable<Bundle> testBundles, string argValue, BundleType typeSelection, BundleArch archSelection, string errorMessage)
            where TException : Exception
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"dry-run --{Option.Name} {argValue}");
            Action action = () => OptionFilterer.Filter(parseResult, Option, testBundles, typeSelection, archSelection);

            action.Should().Throw<TException>(errorMessage);
        }

        private static Bundle<TBundleVersion> GetBundleFromInput<TBundleVersion>(string input, BundleArch arch)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>, new()
        {
            var version = BundleVersion.FromInput<TBundleVersion>(input);
            return Bundle.From(version, arch, TestUninstallCommand, TestDisplayName) as Bundle<TBundleVersion>;
        }
    }
}
