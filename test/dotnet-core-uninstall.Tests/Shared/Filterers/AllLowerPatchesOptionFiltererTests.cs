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
        internal override string DefaultTestArgValue => "";

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
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
                new List<Bundle>
                {
                    Runtime_3_0_0_P_X86,
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
                    Runtime_3_0_0_P_X86,
                    Runtime_2_2_4_X86
                },
                BundleType.Runtime,
                BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>(),
                BundleType.Sdk,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    AspNetRuntime_3_0_0_P_X64,
                    AspNetRuntime_2_2_3_X64
                },
                BundleType.AspNetRuntime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                new List<Bundle>
                {
                    HostingBundle_3_0_0_P4_X86,
                    HostingBundle_2_2_0_X86,
                    HostingBundle_2_2_0_P3_X86,
                    HostingBundle_2_2_0_P1_X86
                },
                BundleType.HostingBundle,
                DefaultTestArchSelection
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllLowerPatchesOptionFiltererGood(IEnumerable<Bundle> testBundles, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, DefaultTestArgValue, expected, typeSelection, archSelection);
        }
    }
}
