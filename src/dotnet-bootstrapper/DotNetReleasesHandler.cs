using System;  
using Microsoft.Deployment.DotNet.Releases;

namespace Microsoft.DotNet.Tools.Bootstrapper;

internal static class DotNetReleasesHandler
{
    internal static Uri[] DotNetReleasesIndexFeeds = new Uri[]
    {
            new("https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json"),
            new("https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/2.1/releases.json")
    };

    internal static Uri DotNetRelesesMetadataFeed(string channelVersion) =>
        new($"https://builds.dotnet.microsoft.com/dotnet/release-metadata/{channelVersion}/releases.json");
}
