using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DotNet.Tools.Bootstrapper.Commands
{
    internal static class Common
    {
        internal static Option<bool> AllowPreviewsOptions = new Option<bool>(
            "--allow-previews",
            description: "Include pre-release sdk versions");
    }
}
