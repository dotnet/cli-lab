// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading.Tasks;
namespace Microsoft.DotNet.Tools.Bootstrapper;

class Program
{
    static int Main(string[] args)
    {
        var rootCommand = new RootCommand("dotnet bootstrapper");

        var installCommand = new Command("install", "Install the specified version of the .NET SDK.")
        {
            new Argument<string>("version")
            {
                Description = "The version of the .NET SDK to install.",
                Arity = ArgumentArity.ExactlyOne
            }
        };

        rootCommand.AddCommand(installCommand);

        return rootCommand.Invoke(args);
    }
}
