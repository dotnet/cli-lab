using System;
using System.Collections.Generic;
using System.CommandLine;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Filterers;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public class NoOptionFiltererTests : FiltererTests
    {
        internal override Option Option => null;
        internal override string DefaultArgValue => "2.2.5";
        internal override bool TestBundleTypeNotSpecifiedException => true;
        internal override Filterer OptionFilterer => OptionFilterers.UninstallNoOptionFilterer;

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202",
                new List<Bundle>
                {
                    Sdk_2_2_202_X64,
                    Sdk_2_2_202_X86
                },
                BundleType.Sdk
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5",
                new List<Bundle>
                {
                    Runtime_2_2_5_X64,
                    Runtime_2_2_5_X86
                },
                BundleType.Runtime
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202 2.1.300-rc1-008673",
                new List<Bundle>
                {
                    Sdk_2_2_202_X64,
                    Sdk_2_2_202_X86,
                    Sdk_2_1_300_Rc1_X64,
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5 3.0.0-preview5-27626-15",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P5_X86,
                    Runtime_2_2_5_X64,
                    Runtime_2_2_5_X86
                },
                BundleType.Runtime
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202 2.2.202 2.2.202",
                new List<Bundle>
                {
                    Sdk_2_2_202_X64,
                    Sdk_2_2_202_X86
                },
                BundleType.Sdk
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5 2.1.0-rc1 3.0.0-preview5-27626-15 3.0.0-preview5-27626-15 2.1.0-rc1 2.1.0-rc1",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P5_X86,
                    Runtime_2_2_5_X64,
                    Runtime_2_2_5_X86,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestNoOptionFiltererGood(IEnumerable<Bundle> testBundles, string argValue, IEnumerable<Bundle> expected, BundleType typeSelection)
        {
            TestFiltererGood(testBundles, argValue, expected, typeSelection);
        }

        [Theory]
        [InlineData("2.2.302", BundleType.Sdk)]
        [InlineData("2.2.3", BundleType.Runtime)]
        [InlineData("2.2.202 2.2.302 2.1.520", BundleType.Sdk)]
        [InlineData("2.2.1 2.2.2 2.2.3 2.2.4 2.2.5", BundleType.Runtime)]
        [InlineData("3.0.100-preview2-011568", BundleType.Sdk)]
        [InlineData("2.1.300-rc3-008673", BundleType.Sdk)]
        [InlineData("3.0.0-preview4-27626-15", BundleType.Runtime)]
        [InlineData("3.0.0-preview5-27626-16", BundleType.Runtime)]
        [InlineData("2.1.0-rc2", BundleType.Runtime)]
        [InlineData("2.1.0-rc1-008673", BundleType.Runtime)]
        [InlineData("2.2.5", BundleType.Sdk)]
        [InlineData("2.2.202", BundleType.Runtime)]
        internal void TestNoOptionFiltererSpecifiedVersionNotFoundException(string argValue, BundleType typeSelection)
        {
            TestFiltererException<SpecifiedVersionNotFoundException>(DefaultTestBundles, argValue, typeSelection);
        }

        internal override void TestFiltererGood(IEnumerable<Bundle> testBundles, string argValue, IEnumerable<Bundle> expected, BundleType typeSelection)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"{argValue}");

            (OptionFilterer as ArgFilterer<IEnumerable<string>>)
                .Filter(parseResult.RootCommandResult.Arguments, testBundles, typeSelection)
                .Should().BeEquivalentTo(expected);
        }

        internal override void TestFiltererException<TException>(IEnumerable<Bundle> testBundles, string argValue, BundleType typeSelection)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"{argValue}");

            Action action = () => (OptionFilterer as ArgFilterer<IEnumerable<string>>)
                .Filter(parseResult.RootCommandResult.Arguments, testBundles, typeSelection);

            action.Should().Throw<TException>();
        }
    }
}
