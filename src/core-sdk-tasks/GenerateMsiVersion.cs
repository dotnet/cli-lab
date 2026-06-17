// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.DotNet.Cli.Build
{
    public class GenerateMsiVersion : Task
    {
        [Required]
        public int BuildNumber { get; set; }

        [Required]
        public int Major { get; set; }

        [Required]
        public int Minor { get; set; }

        [Required]
        public int Patch { get; set; }

        [Output]
        public string MsiVersion { get; set; }

        public override bool Execute()
        {
            var buildVersion = new Version()
            {
                Major = Major,
                Minor = Minor,
                Patch = Patch,
                CommitCount = BuildNumber
            };

            MsiVersion = buildVersion.GenerateMsiVersion();

            return true;
        }
    }
}
