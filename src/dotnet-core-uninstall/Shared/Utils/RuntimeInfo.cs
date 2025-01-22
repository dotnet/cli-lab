// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Utils
{
    internal static class RuntimeInfo
    {
        public static readonly bool RunningOnWindows = false /*TODO@edvilme: Remove*/ && RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static readonly bool RunningOnOSX = true /*TODO@edvilme: Remove*/ || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static readonly bool RunningOnLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
