// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Tests.Attributes;
using Microsoft.DotNet.Tools.Uninstall.Windows;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Windows
{
    public class RegistryQueryTests
    {
        [WindowsOnlyTheory]
        [InlineData("Microsoft .NET Core SDK 2.2.202 (x64)")]
        [InlineData("Microsoft .NET Core SDK 2.1.300 - rc1 (x86)")]
        [InlineData("Microsoft .NET Core SDK 3.0.100 - preview5 (x64)")]
        [InlineData(".NET Core SDK 1.1.10 (x64)")]
        [InlineData("Microsoft .NET Core SDK - 2.1.4 (x86)")]
        [InlineData("Microsoft .NET Core Runtime - 2.2.4 (x64)")]
        [InlineData("Microsoft .NET Core Runtime - 2.1.0 Release Candidate 1 (x86)")]
        [InlineData("Microsoft .NET Core Runtime - 3.0.0 Preview 5 (x64)")]
        [InlineData("Microsoft .NET Core 1.1.13 - Runtime (x64)")]
        [InlineData("Microsoft ASP.NET Core 3.0.0 Preview 6 Build 19307.2 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.2.6 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.2.0 Preview 3 Build 35497 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.1.0 Release Candidate 1 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.1.0 Preview 2 - Shared Framework")]
        [InlineData("Microsoft ASP.NET Core 2.0.9 - Runtime Package Store")]
        [InlineData("Microsoft ASP.NET Core 3.0.0 Preview Build 18579-0056 - Shared Framework")]
        [InlineData("Microsoft .NET Core 1.0.9 & 1.1.6 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 1.0.16 & 1.1.13 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.0.3 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core  Preview 1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core  Release Candidate 1 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.1.12 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 1 35029 preview1 - Windows Server Hosting - 35029")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 2 35157 35157 preview2 - Windows Server Hosting - 35157")]
        [InlineData("Microsoft .NET Core 2.2.0 Preview 3 35497 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.2.0 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 2.2.6 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview Build 18579-0056 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview Build 19075-0444 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview 4 Build 19216-03 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview 5 Build 19227-01 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.0.0 Preview 6 Build 19307.2 - Windows Server Hosting")]
        [InlineData("Microsoft .NET Core 3.1.0 Preview 1 Build preview1.19307.20 - Windows Server Hosting")]
        [InlineData("Microsoft .NET 5.0.9 - Windows Server Hosting")]
        internal void TestIsNetCoreBundleAccept(string input)
        {
            RegistryQuery.IsNetCoreBundle(input, "0.0", "mockuninstall.exe", "0.0")
                .Should()
                .BeTrue();
        }

        [WindowsOnlyTheory]
        [InlineData("Microsoft ASP.NET Web Frameworks and Tools VS2015")]
        [InlineData("Microsoft .NET Core SDK - rc1 (x86)")]
        internal void TestGetBundleVersionReturnsNullOnInvalidDisplayNames(string displayName)
        {
            RegistryQuery.GetBundleVersion(displayName, string.Empty, string.Empty)
                .Should()
                .BeNull();
        }
    }
}
