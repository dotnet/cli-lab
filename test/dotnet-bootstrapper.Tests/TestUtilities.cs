// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.DotNet.Tools.Bootstrapper.Tests
{
    static class TestUtilities
    {
        public static string GetRuntimeIdentifier()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return RuntimeInformation.OSArchitecture switch
                {
                    Architecture.X64 => "win-x64",
                    Architecture.X86 => "win-x86",
                    Architecture.Arm => "win-arm",
                    Architecture.Arm64 => "win-arm64",
                    _ => throw new NotSupportedException("Unsupported architecture on Windows")
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return RuntimeInformation.OSArchitecture switch
                {
                    Architecture.X64 => "linux-x64",
                    Architecture.Arm => "linux-arm",
                    Architecture.Arm64 => "linux-arm64",
                    _ => throw new NotSupportedException("Unsupported architecture on Linux")
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return RuntimeInformation.OSArchitecture switch
                {
                    Architecture.X64 => "osx-x64",
                    Architecture.Arm64 => "osx-arm64",
                    _ => throw new NotSupportedException("Unsupported architecture on macOS")
                };
            }

            throw new NotSupportedException("Unsupported operating system");
        }
    }
}
