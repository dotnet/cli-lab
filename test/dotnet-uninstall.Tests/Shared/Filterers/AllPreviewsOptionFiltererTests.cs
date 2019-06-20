using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public class AllPreviewsOptionFiltererTests : FiltererTests
    {
        internal override Option Option => CommandLineConfigs.UninstallAllPreviewsOption;
        internal override string DefaultArgValue => "";
        internal override bool TestBundleTypeNotSpecifiedException => false;

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_1_300_Rc1_Arm32,
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
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P5_X86,
                    Runtime_3_0_0_P_Arm32,
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
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_1_300_Rc1_Arm32,
                    Sdk_2_1_300_Rc1_X86,
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P5_X86,
                    Runtime_3_0_0_P_Arm32,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Sdk | BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_2_1_300_Rc1_Arm32
                },
                BundleType.Sdk,
                BundleArch.Arm32
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X86,
                    Runtime_3_0_0_P_Arm32
                },
                BundleType.Runtime,
                BundleArch.Arm32 | BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_1_300_Rc1_X86,
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P5_X86,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Sdk | BundleType.Runtime,
                BundleArch.X64 | BundleArch.X86
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllPreviewsOptionFiltererGood(IEnumerable<Bundle> testBundles, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, DefaultArgValue, expected, typeSelection, archSelection);
        }
    }
}
