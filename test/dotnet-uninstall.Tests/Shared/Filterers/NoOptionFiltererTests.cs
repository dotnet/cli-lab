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
        internal override string DefaultTestArgValue => "2.2.5";
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
                BundleType.Sdk,
                DefaultTestArchSelection
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
                BundleType.Runtime,
                DefaultTestArchSelection
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
                BundleType.Sdk,
                DefaultTestArchSelection
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
                BundleType.Runtime,
                DefaultTestArchSelection
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
                BundleType.Sdk,
                DefaultTestArchSelection
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
                BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5",
                new List<Bundle>
                {
                    Runtime_2_2_5_X64
                },
                BundleType.Runtime,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202 2.1.300-rc1-008673",
                new List<Bundle>
                {
                    Sdk_2_2_202_X86,
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk,
                BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5 3.0.0-preview5-27626-15",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X86,
                    Runtime_2_2_5_X86
                },
                BundleType.Runtime,
                BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202 2.2.202 2.2.202",
                new List<Bundle>
                {
                    Sdk_2_2_202_X64
                },
                BundleType.Sdk,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5 2.1.0-rc1 3.0.0-preview5-27626-15 3.0.0-preview5-27626-15 2.1.0-rc1 2.1.0-rc1",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_2_2_5_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime,
                BundleArch.X64
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestNoOptionFiltererGood(IEnumerable<Bundle> testBundles, string argValue, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, argValue, expected, typeSelection, archSelection);
        }

        [Theory]
        [InlineData("2.2.302", BundleType.Sdk, DefaultTestArchSelection)]
        [InlineData("2.2.3", BundleType.Runtime, DefaultTestArchSelection)]
        [InlineData("2.2.202 2.2.302 2.1.520", BundleType.Sdk, DefaultTestArchSelection)]
        [InlineData("2.2.1 2.2.2 2.2.3 2.2.4 2.2.5", BundleType.Runtime, DefaultTestArchSelection)]
        [InlineData("3.0.100-preview2-011568", BundleType.Sdk, DefaultTestArchSelection)]
        [InlineData("2.1.300-rc3-008673", BundleType.Sdk, DefaultTestArchSelection)]
        [InlineData("3.0.0-preview4-27626-15", BundleType.Runtime, DefaultTestArchSelection)]
        [InlineData("3.0.0-preview5-27626-16", BundleType.Runtime, DefaultTestArchSelection)]
        [InlineData("2.1.0-rc2", BundleType.Runtime, DefaultTestArchSelection)]
        [InlineData("2.1.0-rc1-008673", BundleType.Runtime, DefaultTestArchSelection)]
        [InlineData("2.2.5", BundleType.Sdk, DefaultTestArchSelection)]
        [InlineData("2.2.202", BundleType.Runtime, DefaultTestArchSelection)]
        [InlineData("2.2.222", BundleType.Sdk, BundleArch.X64)]
        [InlineData("3.0.100-preview5-011568", BundleType.Sdk, BundleArch.X86 | BundleArch.X86)]
        [InlineData("2.2.4", BundleType.Runtime, BundleArch.X64)]
        [InlineData("2.1.0-rc1", BundleType.Runtime, BundleArch.X86)]
        internal void TestNoOptionFiltererSpecifiedVersionNotFoundException(string argValue, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererException<SpecifiedVersionNotFoundException>(DefaultTestBundles, argValue, typeSelection, archSelection, string.Format(LocalizableStrings.SpecifiedVersionNotFoundExceptionMessageFormat, argValue));
        }

        internal override void TestFiltererGood(IEnumerable<Bundle> testBundles, string argValue, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"{argValue}");

            (OptionFilterer as ArgFilterer<IEnumerable<string>>)
                .Filter(parseResult.RootCommandResult.Arguments, testBundles, typeSelection, archSelection)
                .Should().BeEquivalentTo(expected);
        }

        internal override void TestFiltererException<TException>(IEnumerable<Bundle> testBundles, string argValue, BundleType typeSelection, BundleArch archSelection, string errorMessage = null)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"{argValue}");

            Action action = () => (OptionFilterer as ArgFilterer<IEnumerable<string>>)
                .Filter(parseResult.RootCommandResult.Arguments, testBundles, typeSelection, archSelection);

            if (errorMessage == null)
            {
                action.Should().Throw<TException>();
            }
            else
            {
                action.Should().Throw<TException>(errorMessage);
            }
        }
    }
}
