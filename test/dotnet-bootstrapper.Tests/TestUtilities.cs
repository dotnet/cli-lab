// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.DotNet.Tools.Bootstrapper.Tests
{
    static class TestUtilities
    {
        public static string GetTargetFramework()
        {
            return "net8.0";
        }
        
        public static string GetConfiguration()
        {
#if DEBUG
            return "Debug";
#elif RELEASE
            return "Release";
#else
            return "Custom";
#endif
        }

        public static string GetRuntimeIdentifier()
        {
            return RuntimeInformation.RuntimeIdentifier;
        }
    }
}
