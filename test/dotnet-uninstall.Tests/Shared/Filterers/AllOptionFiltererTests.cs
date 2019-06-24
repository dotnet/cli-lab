using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public class AllOptionFiltererTests : FiltererTests
    {
        internal override Option Option => CommandLineConfigs.UninstallAllOption;
        internal override string DefaultArgValue => "";

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
                DefaultTestSdks,
                BundleType.Sdk,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                DefaultTestRuntimes,
                BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle<SdkVersion>>
                {
                    Sdk_2_1_700_X64,
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64
                },
                BundleType.Sdk,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle<RuntimeVersion>>
                {
                    Runtime_2_2_4_X86,
                    Runtime_2_2_5_X86,
                    Runtime_3_0_0_P5_X86,
                    Runtime_2_2_5_Arm32,
                    Runtime_3_0_0_P_Arm32
                },
                BundleType.Runtime,
                BundleArch.Arm32 | BundleArch.X86
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllOptionFiltererGood(IEnumerable<Bundle> testBundles, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, DefaultArgValue, expected, typeSelection, archSelection);
        }
    }
}
