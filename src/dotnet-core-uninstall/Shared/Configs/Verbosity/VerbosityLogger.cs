// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity
{
    internal class VerbosityLogger
    {
        public VerbosityLevel Level { get; set; }

        public VerbosityLogger(VerbosityLevel level)
        {
            if (IsOutOfRange(level))
            {
                throw new ArgumentOutOfRangeException();
            }

            Level = level;
        }

        public void Log(VerbosityLevel logLevel, string message)
        {
            if (logLevel == VerbosityLevel.Quiet || IsOutOfRange(logLevel))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (Level >= logLevel)
            {
                Console.WriteLine(message);
            }
        }

        private bool IsOutOfRange(VerbosityLevel level)
        {
            return (int)level >= Enum.GetValues(typeof(VerbosityLevel)).Length;
        }
    }
}
