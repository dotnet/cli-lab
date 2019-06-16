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
        internal override bool TestBundleTypeNotSpecifiedException => false;

        public static IEnumerable<object[]> GetDataForTestFiltererGood()
        {
            yield return new object[]
            {
                TestSdks,
                BundleType.Sdk
            };

            yield return new object[]
            {
                TestRuntimes,
                BundleType.Runtime
            };

            yield return new object[]
            {
                TestBundles,
                BundleType.Sdk | BundleType.Runtime
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFiltererGood))]
        internal void TestAllOptionFiltererGood(IEnumerable<Bundle> expected, BundleType typeSelection)
        {
            TestFiltererGood(DefaultArgValue, expected, typeSelection);
        }
    }
}
