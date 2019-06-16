using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public class AllButLatestOptionFiltererTests : FiltererTests
    {
        internal override Option Option => CommandLineConfigs.UninstallAllButLatestOption;
        internal override string DefaultArgValue => "";
        internal override bool TestBundleTypeNotSpecifiedException => false;

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                new List<Bundle>
                {
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
                new List<Bundle>
                {
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
                new List<Bundle>
                {
                    Sdk_2_2_300_X64,
                    Sdk_2_2_222_X64,
                    Sdk_2_2_202_X64,
                    Sdk_2_2_202_X86,
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X64,
                    Sdk_2_1_300_Rc1_X86,
                    Runtime_3_0_0_P_X86,
                    Runtime_2_2_5_X64,
                    Runtime_2_2_5_X86,
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Sdk | BundleType.Runtime
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllButLatestOptionFiltererGood(IEnumerable<Bundle> expected, BundleType typeSelection)
        {
            TestFiltererGood(DefaultArgValue, expected, typeSelection);
        }
    }
}
