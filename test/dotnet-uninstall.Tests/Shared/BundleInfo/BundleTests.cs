using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Tests.TestUtils;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo
{
    public class BundleTests
    {
        private static readonly SdkVersion TestSdkVersion1 = new SdkVersion("3.0.100-preview5-011568");
        private static readonly SdkVersion TestSdkVersion2 = new SdkVersion("2.1.700");
        private static readonly RuntimeVersion TestRuntimeVersion1 = new RuntimeVersion("2.2.5");
        private static readonly RuntimeVersion TestRuntimeVersion2 = new RuntimeVersion("3.0.0-preview5-27122-01");
        private static readonly string TestUninstallCommand1 = "C:\\ProgramData\\Package Cache\\{a05f1bee-210e-401f-9e98-d52a4698bc91}\\dotnet-sdk-2.2.300-win-x64.exe\" /uninstall /quiet";
        private static readonly string TestUninstallCommand2 = "some random uninstall command";
        private static readonly string TestDisplayName1 = "Microsoft .NET Core SDK 2.2.300 (x64)";
        private static readonly string TestDisplayName2 = "some random display name";

        [Fact]
        internal void TestConstructor()
        {
            var version = TestRuntimeVersion1;
            var uninstallCommand = TestUninstallCommand1;
            var displayName = TestDisplayName1;

            var bundle = new Bundle<RuntimeVersion>(version, BundleArch.X64, uninstallCommand, displayName);

            bundle.Version.Should().Be(version);
            bundle.Arch.Should().Be(BundleArch.X64);
            bundle.UninstallCommand.Should().Be(uninstallCommand);
            bundle.DisplayName.Should().Be(displayName);
        }

        public static IEnumerable<object[]> GetDataForTestConstructorNull()
        {
            yield return new object[]
            {
                null,
                BundleArch.X64,
                TestUninstallCommand1,
                TestDisplayName1
            };

            yield return new object[]
            {
                TestRuntimeVersion1,
                BundleArch.X86,
                null,
                TestDisplayName1
            };

            yield return new object[]
            {
                TestRuntimeVersion1,
                BundleArch.X64,
                TestUninstallCommand1,
                null
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestConstructorNull))]
        internal void TestConstructorNull(RuntimeVersion version, BundleArch arch, string uninstallCommand, string displayName)
        {
            Action action = () => new Bundle<RuntimeVersion>(version, arch, uninstallCommand, displayName);

            action.Should().Throw<ArgumentNullException>();
        }

        public static IEnumerable<object[]> GetDataForTestFrom()
        {
            yield return new object[]
            {
                TestSdkVersion1,
                BundleArch.X64,
                TestUninstallCommand1,
                TestDisplayName1
            };

            yield return new object[]
            {
                TestRuntimeVersion1,
                BundleArch.X64,
                TestUninstallCommand1,
                TestDisplayName1
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFrom))]
        internal void TestFrom<TBundleVersion>(TBundleVersion version, BundleArch arch, string uninstallCommand, string displayName)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>
        {
            var bundle = Bundle.From(version, arch, uninstallCommand, displayName);

            bundle.Should().BeOfType<Bundle<TBundleVersion>>();
            bundle.Version.Type.Should().Be(version.Type);
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new Bundle<RuntimeVersion>(TestRuntimeVersion1, BundleArch.X86, TestUninstallCommand1, TestDisplayName1),
                new Bundle<RuntimeVersion>(TestRuntimeVersion1, BundleArch.X86, TestUninstallCommand1, TestDisplayName1)
            };

            yield return new object[]
            {
                new Bundle<RuntimeVersion>(TestRuntimeVersion2, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                new Bundle<RuntimeVersion>(TestRuntimeVersion2, BundleArch.Arm32, TestUninstallCommand2, TestDisplayName1)
            };

            yield return new object[]
            {
                new Bundle<SdkVersion>(TestSdkVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1),
                new Bundle<SdkVersion>(TestSdkVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1)
            };

            yield return new object[]
            {
                new Bundle<SdkVersion>(TestSdkVersion2, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                new Bundle<SdkVersion>(TestSdkVersion2, BundleArch.Arm32, TestUninstallCommand2, TestDisplayName1)
            };

            yield return new object[]
            {
                new Bundle<SdkVersion>(TestSdkVersion2, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                new Bundle<SdkVersion>(TestSdkVersion2, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName2)
            };

            yield return new object[]
            {
                new Bundle<SdkVersion>(TestSdkVersion2, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                new Bundle<SdkVersion>(TestSdkVersion2, BundleArch.Arm32, TestUninstallCommand2, TestDisplayName2)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestEquality))]
        internal void TestEquality<TBundleVersion>(Bundle<TBundleVersion> bundle1, Bundle<TBundleVersion> bundle2)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>
        {
            EqualityComparisonTestUtils<Bundle<TBundleVersion>>.TestEquality(bundle1, bundle2);
        }

        public static IEnumerable<object[]> GetDataForTestToString()
        {
            yield return new object[]
            {
                TestRuntimeVersion1,
                BundleArch.X64
            };

            yield return new object[]
            {
                TestRuntimeVersion1,
                BundleArch.X86
            };

            yield return new object[]
            {
                TestRuntimeVersion1,
                BundleArch.Arm32
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestToString))]
        internal void TestToString(RuntimeVersion version, BundleArch arch)
        {
            new Bundle<RuntimeVersion>(version, arch, TestUninstallCommand1, TestDisplayName1).ToString()
                .Should().Be($"{version.ToString()} ({arch.ToString().ToLower()})");
        }

        public static IEnumerable<object[]> GetDataForTestInequality()
        {
            yield return new object[]
            {
                Bundle.From(new RuntimeVersion("3.0.0-preview5-27122-01"), BundleArch.X64, TestUninstallCommand1, TestDisplayName1),
                Bundle.From(new RuntimeVersion("3.0.0-preview5-27626-15"), BundleArch.X64, TestUninstallCommand1, TestDisplayName1)
            };

            yield return new object[]
            {
                Bundle.From(new SdkVersion("2.1.700"), BundleArch.X64, TestUninstallCommand1, TestDisplayName1),
                Bundle.From(new SdkVersion("2.2.300"), BundleArch.X64, TestUninstallCommand1, TestDisplayName1)
            };

            yield return new object[]
            {
                Bundle.From(TestRuntimeVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                Bundle.From(TestRuntimeVersion1, BundleArch.X86, TestUninstallCommand1, TestDisplayName1)
            };

            yield return new object[]
            {
                Bundle.From(TestRuntimeVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                Bundle.From(TestRuntimeVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1)
            };

            yield return new object[]
            {
                Bundle.From(TestRuntimeVersion1, BundleArch.X86, TestUninstallCommand1, TestDisplayName1),
                Bundle.From(TestRuntimeVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1)
            };

            yield return new object[]
            {
                Bundle.From(TestSdkVersion1, BundleArch.X86, TestUninstallCommand1, TestDisplayName1),
                Bundle.From(TestSdkVersion1, BundleArch.X64, TestUninstallCommand2, TestDisplayName1)
            };

            yield return new object[]
            {
                Bundle.From(TestSdkVersion1, BundleArch.X86, TestUninstallCommand2, TestDisplayName1),
                Bundle.From(TestSdkVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequality))]
        internal void TestInequality<TBundleVersion>(Bundle<TBundleVersion> lower, Bundle<TBundleVersion> higher)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>
        {
            EqualityComparisonTestUtils<Bundle<TBundleVersion>>.TestInequality(lower, higher);
        }

        public static IEnumerable<object[]> GetDataForTestInequalityNull()
        {
            yield return new object[]
            {
                Bundle.From(TestRuntimeVersion1, BundleArch.X86, TestUninstallCommand1, TestDisplayName1)
            };

            yield return new object[]
            {
                Bundle.From(TestSdkVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1)
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestInequalityNull))]
        internal void TestInequalityNull<TBundleVersion>(Bundle<TBundleVersion> bundle)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>
        {
            EqualityComparisonTestUtils<Bundle<TBundleVersion>>.TestInequalityNull(bundle);
        }

        public static IEnumerable<object[]> GetDataForTestFilterWithSameBundleType()
        {
            yield return new object[]
            {
                new List<Bundle>
                {
                    Bundle.From(TestRuntimeVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1),
                    Bundle.From(TestSdkVersion1, BundleArch.X86, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestSdkVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestSdkVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                    Bundle.From(TestRuntimeVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestSdkVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestRuntimeVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1)
                },
                new List<Bundle<RuntimeVersion>>
                {
                    Bundle.From(TestRuntimeVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1) as Bundle<RuntimeVersion>,
                    Bundle.From(TestRuntimeVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1) as Bundle<RuntimeVersion>,
                    Bundle.From(TestRuntimeVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1) as Bundle<RuntimeVersion>
                }
            };

            yield return new object[]
            {
                new List<Bundle>
                {
                    Bundle.From(TestRuntimeVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1),
                    Bundle.From(TestRuntimeVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestRuntimeVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1)
                },
                new List<Bundle<RuntimeVersion>>
                {
                    Bundle.From(TestRuntimeVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1) as Bundle<RuntimeVersion>,
                    Bundle.From(TestRuntimeVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1) as Bundle<RuntimeVersion>,
                    Bundle.From(TestRuntimeVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1) as Bundle<RuntimeVersion>
                }
            };

            yield return new object[]
            {
                new List<Bundle>
                {
                    Bundle.From(TestSdkVersion1, BundleArch.X86, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestSdkVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestSdkVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                    Bundle.From(TestSdkVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1)
                },
                new List<Bundle<RuntimeVersion>>()
            };

            yield return new object[]
            {
                new List<Bundle>
                {
                    Bundle.From(TestRuntimeVersion1, BundleArch.X64, TestUninstallCommand1, TestDisplayName1),
                    Bundle.From(TestRuntimeVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestRuntimeVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1)
                },
                new List<Bundle<SdkVersion>>()
            };

            yield return new object[]
            {
                new List<Bundle>
                {
                    Bundle.From(TestSdkVersion1, BundleArch.X86, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestSdkVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1),
                    Bundle.From(TestSdkVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1),
                    Bundle.From(TestSdkVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1)
                },
                new List<Bundle<SdkVersion>>
                {
                    Bundle.From(TestSdkVersion1, BundleArch.X86, TestUninstallCommand2, TestDisplayName1) as Bundle<SdkVersion>,
                    Bundle.From(TestSdkVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1) as Bundle<SdkVersion>,
                    Bundle.From(TestSdkVersion1, BundleArch.Arm32, TestUninstallCommand1, TestDisplayName1) as Bundle<SdkVersion>,
                    Bundle.From(TestSdkVersion2, BundleArch.X64, TestUninstallCommand2, TestDisplayName1) as Bundle<SdkVersion>
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFilterWithSameBundleType))]
        internal void TestFilterWithSameBundleType<TBundleVersion>(IEnumerable<Bundle> bundles, IEnumerable<Bundle<TBundleVersion>> expected)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>
        {
            Bundle<TBundleVersion>.FilterWithSameBundleType(bundles)
                .Should().BeEquivalentTo(expected);
        }
    }
}
