using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public class AllPreviewsButLatestOptionFiltererTests : FiltererTests
    {
        internal override Option Option => CommandLineConfigs.UninstallAllPreviewsButLatestOption;
        internal override string DefaultTestArgValue => "";

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
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
                    Runtime_3_0_0_P_Arm32,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>(),
                BundleType.Sdk,
                BundleArch.Arm32 | BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>(),
                BundleType.Runtime,
                BundleArch.Arm32
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_2_1_300_Rc1_X86
                },
                BundleType.Sdk,
                BundleArch.X64 | BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Runtime_3_0_0_P_Arm32
                },
                BundleType.Runtime,
                BundleArch.Arm32 | BundleArch.X86
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllPreviewsButLatestOptionFiltererGood(IEnumerable<Bundle> testBundles, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, DefaultTestArgValue, expected, typeSelection, archSelection);
        }
    }
}
