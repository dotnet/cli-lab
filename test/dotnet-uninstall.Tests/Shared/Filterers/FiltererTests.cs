﻿using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Filterers;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public abstract class FiltererTests
    {
        internal abstract Option Option { get; }
        internal abstract string DefaultTestArgValue { get; }
        internal virtual Filterer OptionFilterer => OptionFilterers.OptionFiltererDictionary[Option];

        internal const BundleArch DefaultTestArchSelection = BundleArch.Arm32 | BundleArch.X86 | BundleArch.X64;

        internal static Bundle<SdkVersion> Sdk_3_0_100_P5_X64 = GetBundleFromInput<SdkVersion>("3.0.100-preview5-011568", BundleArch.X64);
        internal static Bundle<SdkVersion> Sdk_2_2_300_X64 = GetBundleFromInput<SdkVersion>("2.2.300", BundleArch.X64);
        internal static Bundle<SdkVersion> Sdk_2_2_222_X86 = GetBundleFromInput<SdkVersion>("2.2.222", BundleArch.X86);
        internal static Bundle<SdkVersion> Sdk_2_2_202_Arm32 = GetBundleFromInput<SdkVersion>("2.2.202", BundleArch.Arm32);
        internal static Bundle<SdkVersion> Sdk_2_2_202_X86 = GetBundleFromInput<SdkVersion>("2.2.202", BundleArch.X86);
        internal static Bundle<SdkVersion> Sdk_2_1_700_X64 = GetBundleFromInput<SdkVersion>("2.1.700", BundleArch.X64);
        internal static Bundle<SdkVersion> Sdk_2_1_300_Rc1_Arm32 = GetBundleFromInput<SdkVersion>("2.1.300-rc1-008673", BundleArch.Arm32);
        internal static Bundle<SdkVersion> Sdk_2_1_300_Rc1_X86 = GetBundleFromInput<SdkVersion>("2.1.300-rc1-008673", BundleArch.X86);

        internal static Bundle<RuntimeVersion> Runtime_3_0_0_P5_X64 = GetBundleFromInput<RuntimeVersion>("3.0.0-preview5-27626-15", BundleArch.X64);
        internal static Bundle<RuntimeVersion> Runtime_3_0_0_P5_X86 = GetBundleFromInput<RuntimeVersion>("3.0.0-preview5-27626-15", BundleArch.X86);
        internal static Bundle<RuntimeVersion> Runtime_3_0_0_P_Arm32 = GetBundleFromInput<RuntimeVersion>("3.0.0-preview-27122-01", BundleArch.Arm32);
        internal static Bundle<RuntimeVersion> Runtime_2_2_5_Arm32 = GetBundleFromInput<RuntimeVersion>("2.2.5", BundleArch.Arm32);
        internal static Bundle<RuntimeVersion> Runtime_2_2_5_X86 = GetBundleFromInput<RuntimeVersion>("2.2.5", BundleArch.X86);
        internal static Bundle<RuntimeVersion> Runtime_2_2_4_X86 = GetBundleFromInput<RuntimeVersion>("2.2.4", BundleArch.X86);
        internal static Bundle<RuntimeVersion> Runtime_2_2_2_X64 = GetBundleFromInput<RuntimeVersion>("2.2.2", BundleArch.X64);
        internal static Bundle<RuntimeVersion> Runtime_2_1_0_Rc1_X64 = GetBundleFromInput<RuntimeVersion>("2.1.0-rc1", BundleArch.X64);

        internal static readonly IEnumerable<Bundle<SdkVersion>> DefaultTestSdks = new List<Bundle<SdkVersion>>
        {
            Sdk_2_2_202_Arm32,
            Sdk_2_1_300_Rc1_Arm32,
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
            Runtime_2_2_5_Arm32,
            Runtime_3_0_0_P_Arm32,
            Runtime_2_1_0_Rc1_X64
        };

        internal static readonly IEnumerable<Bundle> DefaultTestBundles = DefaultTestSdks
            .Select(sdk => sdk as Bundle)
            .Concat(DefaultTestRuntimes.Select(runtime => runtime as Bundle));

        [Theory]
        [InlineData((BundleType)0)]
        [InlineData((BundleType.Sdk | BundleType.Runtime) + 1)]
        [InlineData((BundleType.Sdk | BundleType.Runtime) + 2)]
        [InlineData((BundleType.Sdk | BundleType.Runtime) + 10)]
        internal void TestFiltererArgumentOutOfRangeException(BundleType typeSelection)
        {
            TestFiltererException<ArgumentOutOfRangeException>(DefaultTestBundles, DefaultTestArgValue, typeSelection, DefaultTestArchSelection);
        }

        [Fact]
        internal void TestFiltererBundleTypeNotSpecifiedException()
        {
            TestFiltererException<BundleTypeMissingException>(DefaultTestBundles, DefaultTestArgValue, BundleType.Sdk | BundleType.Runtime, DefaultTestArchSelection);
        }

        internal virtual void TestFiltererGood(IEnumerable<Bundle> testBundles, string argValue, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"--{Option.Name} {argValue}");

            OptionFilterer.Filter(parseResult, Option, testBundles, typeSelection, archSelection)
                .Should().BeEquivalentTo(expected);
        }

        internal virtual void TestFiltererException<TException>(IEnumerable<Bundle> testBundles, string argValue, BundleType typeSelection, BundleArch archSelection)
            where TException : Exception
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"--{Option.Name} {argValue}");

            Action action = () => OptionFilterer.Filter(parseResult, Option, testBundles, typeSelection, archSelection);
            action.Should().Throw<TException>();
        }

        private static Bundle<TBundleVersion> GetBundleFromInput<TBundleVersion>(string input, BundleArch arch)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>, new()
        {
            return Bundle.From(BundleVersion.FromInput<TBundleVersion>(input), arch, input) as Bundle<TBundleVersion>;
        }
    }
}
