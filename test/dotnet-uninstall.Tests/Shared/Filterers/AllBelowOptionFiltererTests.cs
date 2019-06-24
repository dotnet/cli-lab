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

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202",
                new List<Bundle>
                {
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_Arm32,
                    Sdk_2_1_300_Rc1_X86
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
                "3.0.200",
                new List<Bundle>
                {
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64,
                    Sdk_2_2_222_X86,
                    Sdk_2_2_202_Arm32,
                    Sdk_2_2_202_X86,
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_Arm32,
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "3.1.2",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P5_X86,
                    Runtime_3_0_0_P_Arm32,
                    Runtime_2_2_5_Arm32,
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
                "2.0.100-preview-021378",
                new List<Bundle>(),
                BundleType.Sdk,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "1.0.3",
                new List<Bundle>(),
                BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.1.907",
                new List<Bundle>
                {
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X86,
                    Sdk_2_1_300_Rc1_Arm32
                },
                BundleType.Sdk,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.4-preview5-27626-15",
                new List<Bundle>
                {
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
                "2.2.202",
                new List<Bundle>
                {
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk,
                BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5",
                new List<Bundle>
                {
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "3.0.200",
                new List<Bundle>
                {
                    Sdk_2_2_202_Arm32,
                    Sdk_2_1_300_Rc1_Arm32
                },
                BundleType.Sdk,
                BundleArch.Arm32
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "3.1.2",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P_Arm32,
                    Runtime_2_2_5_Arm32,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime,
                BundleArch.X64 | BundleArch.Arm32
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.0.100-preview-021378",
                new List<Bundle>(),
                BundleType.Sdk,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "1.0.3",
                new List<Bundle>(),
                BundleType.Runtime,
                BundleArch.Arm32 | BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.1.907",
                new List<Bundle>
                {
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk,
                BundleArch.X64 | BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.4-preview5-27626-15",
                new List<Bundle>
                {
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime,
                BundleArch.X64 | BundleArch.Arm32
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllBelowOptionFiltererGood(IEnumerable<Bundle> testBundles, string argValue, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, argValue, expected, typeSelection, archSelection);
        }
    }
}
