using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Filterers
{
    public class AllButOptionFiltererTests : FiltererTests
    {
        internal override Option Option => CommandLineConfigs.UninstallAllButOption;
        internal override string DefaultTestArgValue => "2.2.5";

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202",
                new List<Bundle>
                {
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64,
                    Sdk_2_2_222_X86,
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
                "2.2.5",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_3_0_0_P5_X86,
                    Runtime_3_0_0_P_X86,
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
                "2.2.202 2.1.300-rc1-008673",
                new List<Bundle>
                {
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64,
                    Sdk_2_2_222_X86,
                    Sdk_2_1_700_X64
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
                    Runtime_3_0_0_P_X86,
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
                "2.2.907",
                new List<Bundle>
                {
                    Sdk_3_0_100_P5_X64,
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
                "2.2.7",
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
                BundleType.Runtime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202 2.1.300-rc1-008673 3.0.102",
                new List<Bundle>
                {
                    Sdk_2_2_222_X86,
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64,
                    Sdk_2_1_700_X64
                },
                BundleType.Sdk,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5 2.1.14 3.0.0-preview5-27626-15",
                new List<Bundle>
                {
                    Runtime_3_0_0_P_X86,
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
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64,
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X64
                },
                BundleType.Sdk,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
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
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64,
                    Sdk_2_1_700_X64
                },
                BundleType.Sdk,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5 3.0.0-preview5-27626-15",
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
                "2.2.907",
                new List<Bundle>
                {
                    Sdk_3_0_100_P5_X64,
                    Sdk_2_2_300_X64,
                    Sdk_2_2_202_X64,
                    Sdk_2_1_700_X64,
                    Sdk_2_1_300_Rc1_X64
                },
                BundleType.Sdk,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.7",
                new List<Bundle>
                {
                    Runtime_3_0_0_P5_X64,
                    Runtime_2_2_5_X64,
                    Runtime_2_2_2_X64,
                    Runtime_2_1_0_Rc1_X64
                },
                BundleType.Runtime,
                BundleArch.X64
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.202 2.2.222 2.1.300-rc1-008673 3.0.102",
                new List<Bundle>(),
                BundleType.Sdk,
                BundleArch.X86
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.5 2.1.14 3.0.0-preview5-27626-15",
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
                "2.2.7",
                DefaultTestAspNetRuntimes,
                BundleType.AspNetRuntime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "2.2.6 2.2.3 2.2.6 2.2.3",
                new List<Bundle>
                {
                    AspNetRuntime_3_0_0_P6_X64,
                    AspNetRuntime_3_0_0_P_X64,
                    AspNetRuntime_2_1_0_Rc1_X64
                },
                BundleType.AspNetRuntime,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "3.0.0-preview6.19307.2 3.0.1-preview7-12345-67 1.1.13 2.1.3 1.1.13 2.2.6",
                new List<Bundle>
                {
                    HostingBundle_3_0_0_P4_X86,
                    HostingBundle_2_2_0_X86,
                    HostingBundle_2_2_0_P3_X86,
                    HostingBundle_2_2_0_P1_X86,
                    HostingBundle_2_1_0_Rc1_X86
                },
                BundleType.HostingBundle,
                DefaultTestArchSelection
            };

            yield return new object[]
            {
                DefaultTestBundles,
                "1.2.3 1.3.2 2.1.3 2.3.1 3.1.2 3.2.1",
                DefaultTestHostingBundles,
                BundleType.HostingBundle,
                DefaultTestArchSelection
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllButOptionFiltererGood(IEnumerable<Bundle> testBundles, string argValue, IEnumerable<Bundle> expected, BundleType typeSelection, BundleArch archSelection)
        {
            TestFiltererGood(testBundles, argValue, expected, typeSelection, archSelection);
        }
    }
}
