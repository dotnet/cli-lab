using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public class AllBelowOptionFiltererTests : FiltererTests
    {
        internal override Option Option => CommandLineConfigs.UninstallAllBelowOption;
        internal override string DefaultArgValue => "2.2.5";
        internal override bool TestBundleTypeNotSpecifiedException => true;

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                "2.2.202",
                new List<Bundle>
                {
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X64,
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk
            };

            yield return new object[]
            {
                "2.2.5",
                new List<Bundle>
                {
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime
            };

            yield return new object[]
            {
                "3.0.200",
                new List<Bundle>
                {
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64,
                    Sdk_2_2_222_X64,
                    Sdk_2_2_202_X64,
                    Sdk_2_2_202_X86,
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X64,
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk
            };

            yield return new object[]
            {
                "3.1.2",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P5_X86,
                    Runtime_3_0_0_P_X86,
                    Runtime_2_2_5_X64,
                    Runtime_2_2_5_X86,
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime
            };

            yield return new object[]
            {
                "2.0.100-preview-021378",
                new List<Bundle>(),
                BundleType.Sdk
            };

            yield return new object[]
            {
                "1.0.3",
                new List<Bundle>(),
                BundleType.Runtime
            };

            yield return new object[]
            {
                "2.1.907",
                new List<Bundle>
                {
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X86,
                    Sdk_2_1_300_Rc1_X64
                },
                BundleType.Sdk
            };

            yield return new object[]
            {
                "2.2.4-preview5-27626-15",
                new List<Bundle>
                {
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllBelowOptionFiltererGood(string argValue, IEnumerable<Bundle> expected, BundleType typeSelection)
        {
            TestFiltererGood(argValue, expected, typeSelection);
        }
    }
}
