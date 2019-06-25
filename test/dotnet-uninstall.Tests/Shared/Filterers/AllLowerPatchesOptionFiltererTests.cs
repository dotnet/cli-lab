using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public class AllLowerPatchesOptionFiltererTests : FiltererTests
    {
        internal override Option Option => CommandLineConfigs.UninstallAllLowerPatchesOption;
        internal override string DefaultArgValue => "";
        internal override bool TestBundleTypeNotSpecifiedException => false;

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_2_2_202_Arm32,
                    Sdk_2_2_202_X86
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
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64
                },
                BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_2_2_202_Arm32,
                    Sdk_2_2_202_X86,
                    Runtime_3_0_0_P_Arm32,
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64
                },
                BundleType.Sdk | BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_2_2_202_Arm32,
                    Sdk_2_2_202_X86
                },
                BundleType.Sdk,
                BundleArch.X86 | BundleArch.Arm32
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>(),
                BundleType.Runtime,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    Sdk_2_2_202_Arm32,
                    Sdk_2_2_202_X86,
                    Runtime_3_0_0_P_Arm32,
                    Runtime_2_2_4_X86
                },
                BundleType.Sdk | BundleType.Runtime,
                BundleArch.Arm32 | BundleArch.X86
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllLowerPatchesOptionFiltererGood(IEnumerable<Bundle> testBundles, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, DefaultArgValue, expected, typeSelection, archSelection);
        }
    }
}
