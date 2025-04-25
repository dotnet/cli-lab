using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.DotNet.Releases;
using Spectre.Console;

namespace Microsoft.DotNet.Tools.Bootstrapper.Commands.Search;

internal class SearchCommand(
    ParseResult parseResult) : CommandBase(parseResult)
{
    private string _channel = parseResult.ValueForArgument(SearchCommandParser.ChannelArgument);
    private bool _allowPreviews = parseResult.ValueForOption(SearchCommandParser.AllowPreviews);
    public override int Execute()
    {
        List<Product> productCollection = [.. ProductCollection.GetAsync().Result];
        productCollection = [.. 
            productCollection.Where(product => !product.IsOutOfSupport() && (product.SupportPhase != SupportPhase.Preview || _allowPreviews))];
        
        if (!string.IsNullOrEmpty(_channel))
        {
            productCollection = [.. productCollection.Where(product => product.ProductVersion.Equals(_channel, StringComparison.OrdinalIgnoreCase))];
        }

        foreach (Product product in productCollection)
        {
            string productHeader = $"{product.ProductName} {product.ProductVersion}";
            Console.WriteLine(productHeader);

            Table productMetadataTable = new Table()
                .AddColumn("Version")
                .AddColumn("Release Date")
                .AddColumn("SDK")
                .AddColumn("Runtime")
                .AddColumn("ASP.NET Runtime")
                .AddColumn("Windows Desktop Runtime");

            List<ProductRelease> releases = product.GetReleasesAsync().Result.ToList()
                .Where(relase => !relase.IsPreview || _allowPreviews).ToList();

            foreach (ProductRelease release in releases)
            {
                productMetadataTable.AddRow(
                    release.Version.ToString(), 
                    release.ReleaseDate.ToString("yyyy-MM-dd"), 
                    release.Sdks.FirstOrDefault()?.DisplayVersion ?? "N/A",
                    release.Runtime?.DisplayVersion ?? "N/A", 
                    release.AspNetCoreRuntime?.DisplayVersion ?? "N/A", 
                    release.WindowsDesktopRuntime?.DisplayVersion ?? "N/A");
            }
            AnsiConsole.Write(productMetadataTable);
            Console.WriteLine();
        }

        return 0;
    }
}
