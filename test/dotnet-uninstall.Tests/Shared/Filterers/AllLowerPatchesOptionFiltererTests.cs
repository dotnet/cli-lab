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
                new List<Bundle>
                {
                    Sdk_2_2_202_X64,
                    Sdk_2_2_202_X86
                },
                BundleType.Sdk
            };

            yield return new object[]
            {
                new List<Bundle>
                {
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64
                },
                BundleType.Runtime
            };

            yield return new object[]
            {
                new List<Bundle>
                {
                    Sdk_2_2_202_X64,
                    Sdk_2_2_202_X86,
                    Runtime_2_2_4_X86,
                    Runtime_2_2_2_X64
                },
                BundleType.Sdk | BundleType.Runtime
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllLowerPatchesOptionFiltererGood(IEnumerable<Bundle> expected, BundleType typeSelection)
        {
            TestFiltererGood(DefaultArgValue, expected, typeSelection);
        }
    }
}
