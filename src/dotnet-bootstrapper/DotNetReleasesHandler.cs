using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Deployment.DotNet.Releases;

namespace Microsoft.DotNet.Tools.Bootstrapper;

internal static class DotNetReleasesHandler
{
    public static async Task<Product> GetChannelAsync(string channelVersion)
    {
        ProductCollection productCollection = await ProductCollection.GetAsync();
        return productCollection.FirstOrDefault((Product product) => product.ProductVersion.Equals(channelVersion));
    }
}
