
# Uninstall Tool: `dotnet-core-uninstall`
This repo contains the `dotnet-core-uninstall` tool, a command line tool for uninstalling versions of the .NET Core SDK, Runtime, ASP.NET Core Runtime, and Hosting Bundles. For more information about the use of this tool, see the docs [here](https://aka.ms/dotnet-core-uninstall-docs). To download the latest version of the tool, see [the releases page](https://github.com/dotnet/cli-lab/releases).

## Why isn’t the .NET Core Uninstall Tool a .NET Core Global Tool?
An early prototype of the .NET Core Uninstall Tool was a .NET Core Global Tool. Since .NET Core Global Tools are runtime-dependent applications they use a version of the .NET Core Runtime that is installed on the machine. The purpose of the tool is to uninstall arbitrary .NET Core SDKs and Runtimes, so may attempt to uninstall the version that’s running the current process. When this happened, we didn’t find a way to recover from the error and uninstall other SDKs and Runtimes. We saw this as particularly problematic because users might not be clear what runtimes are in SDKs that are being uninstalled or which runtime is currently being used.

## Contributing
We welcome you to try things out, [file issues](https://github.com/dotnet/cli-lab/issues), make feature requests and join us in design conversations.

## Building the Repository
Run the build script at the root of the repo (build.sh or build.cmd) and provide a [runtime identifier](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) (for example, on windows run build /p:RID=win-x86)

## .NET Foundation
The `dotnet-core-uninstall` tool is a .NET Foundation project. See the .NET home repo to find other .NET-related projects.

## Code of Conduct
This project has adopted the code of conduct defined by the [Contributor Covenant](https://www.contributor-covenant.org/) to clarify expected behavior in our community. For more information, see the [.NET Foundation Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct).

## License
.NET Core (including this repo) is licensed under the MIT license.

# MSBuildBinLogQuery
Work in progress
