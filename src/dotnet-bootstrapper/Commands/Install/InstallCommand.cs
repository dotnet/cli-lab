using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.DotNet.Releases;

namespace Microsoft.DotNet.Tools.Bootstrapper.Commands.Install;

internal class InstallCommand(
    ParseResult parseResult) : CommandBase(parseResult)
{
    private string _version = parseResult.ValueForArgument(InstallCommandParser.VersionArgument);
    private bool _allowPreviews = parseResult.ValueForOption(InstallCommandParser.AllowPreviewsOption);


    public override int Execute()
    {
        // If no channel is specified, use the default channel.
        if (string.IsNullOrEmpty(_version))
        {
            _version = BootstrapperUtilities.GetMajorVersionToInstallInDirectory(
                Environment.CurrentDirectory);
        }

        ProductCollection productCollection = ProductCollection.GetAsync().Result;
        Product product = productCollection
            .FirstOrDefault(p => string.IsNullOrEmpty(_version) || p.ProductVersion.Equals(_version, StringComparison.OrdinalIgnoreCase));

        if (product == null)
        {
            Console.WriteLine($"No product found for channel: {_version}");
            return 1;
        }

        ProductRelease latestRelease = product.GetReleasesAsync().Result
            .Where(release => !release.IsPreview || _allowPreviews)
            .OrderByDescending(release => release.ReleaseDate)
            .FirstOrDefault();

        if (latestRelease == null)
        {
            Console.WriteLine($"No releases found for product: {product.ProductName}");
            return 1;
        }

        Console.WriteLine($"Installing {product.ProductName} {latestRelease.Version}...");

        string installationDirectoryPath = BootstrapperUtilities.GetInstallationDirectoryPath();

        foreach (ReleaseComponent component in latestRelease.Components)
        {
            Console.WriteLine($"Installing {component.Name}...");
            DownloadAndExtractReleaseComponentFiles(component, installationDirectoryPath);
        }

        return 0;
    }

    private static void DownloadAndExtractReleaseComponentFiles(ReleaseComponent component, string basePath)
    {
        if (component is WindowsDesktopReleaseComponent && !OperatingSystem.IsWindows())
        {
            return;
        }

        ReleaseFile releaseFile = component.Files.FirstOrDefault(file =>
            file.Rid.Equals(RuntimeInformation.RuntimeIdentifier, StringComparison.OrdinalIgnoreCase) && (file.Name.EndsWith(".zip") || file.Name.EndsWith(".tar.gz")));

        if (string.IsNullOrEmpty(releaseFile?.FileName))
        {
            Console.WriteLine($"\tNo suitable file found for {component.Name}");
            return;
        }

        string zipPath = Path.Combine(basePath, releaseFile.FileName);

        if (File.Exists(zipPath))
        {
            Console.WriteLine($"\t{component.Name} already exists at {zipPath}");
            return;
        }

        try
        {
            releaseFile.DownloadAsync(zipPath)?.Wait();

            // Extract the downloaded file
            ZipFile.ExtractToDirectory(zipPath, Path.ChangeExtension(zipPath, ""));

            Console.WriteLine($"\tExtracted {component.Name} to {Path.ChangeExtension(zipPath, "")}");

            // Delete the downloaded file
            File.Delete(zipPath);
        }
        catch (IOException)
        {
            return;
        }
    }
}
