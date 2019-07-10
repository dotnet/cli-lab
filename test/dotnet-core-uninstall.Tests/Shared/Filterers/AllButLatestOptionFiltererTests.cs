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
        internal override string DefaultTestArgValue => "";

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_2_2_300_X64,
                    Sdk_2_2_222_X86,
                    Sdk_2_2_202_X64,
                    Sdk_2_2_202_X86,
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X64,
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Runtime_3_0_0_P_X86,
                    Runtime_2_2_5_X64,
                    Runtime_2_2_5_X86,
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
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
                new List<Bundle>
                {
                    Runtime_2_2_5_X64,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime,
                BundleArch.X64
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllButLatestOptionFiltererGood(IEnumerable<Bundle> testBundles, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, DefaultTestArgValue, expected, typeSelection, archSelection);
        }
    }
}
