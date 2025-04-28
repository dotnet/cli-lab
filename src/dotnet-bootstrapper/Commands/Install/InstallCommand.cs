using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.DotNet.Releases;

namespace Microsoft.DotNet.Tools.Bootstrapper.Commands.Install;

internal class InstallCommand(
    ParseResult parseResult) : CommandBase(parseResult)
{
    private string _channel = parseResult.ValueForArgument(InstallCommandParser.ChannelArgument);
    private string _rid = BootstrapperUtilities.GetRID();

    public override int Execute()
    {
        // If no channel is specified, use the default channel.
        if (string.IsNullOrEmpty(_channel))
        {
            _channel = BootstrapperUtilities.GetVersionToInstallInDirectory(
                Environment.CurrentDirectory);
        }

        ProductCollection productCollection = ProductCollection.GetAsync().Result;
        Product product = productCollection
            .FirstOrDefault(p => string.IsNullOrEmpty(_channel) || p.ProductVersion.Equals(_channel, StringComparison.OrdinalIgnoreCase));

        if (product == null)
        {
            Console.WriteLine($"No product found for channel: {_channel}");
            return 1;
        }

        ProductRelease latestRelease = product.GetReleasesAsync().Result
            .Where(release => !release.IsPreview)
            .OrderByDescending(release => release.ReleaseDate)
            .FirstOrDefault();

        if (latestRelease == null)
        {
            Console.WriteLine($"No releases found for product: {product.ProductName}");
            return 1;
        }

        Console.WriteLine($"Installing {product.ProductName} {latestRelease.Version}...");

        foreach (ReleaseComponent component in latestRelease.Components)
        {
            Console.WriteLine($"Installing {component.Name} {component.DisplayVersion}...");

            ReleaseFile releaseFile = component.Files.FirstOrDefault(file => file.Rid.Equals(_rid, StringComparison.OrdinalIgnoreCase));

            string path = Path.Combine(BootstrapperUtilities.GetInstallationDirectoryPath(), releaseFile.FileName);

            releaseFile.DownloadAsync(path)?.Wait();

            Console.WriteLine($"Downloaded {component.Name} {component.DisplayVersion} to {path}");

            // Execute the installer files
            if (releaseFile.IsExecutable)
            {
                Console.WriteLine($"Executing {path}...");
                // System.Diagnostics.Process.Start(path)?.WaitForExit();
            }
            else
            {
                Console.WriteLine($"File {path} is not executable.");
            }
        }

        return 0;
    }
}
