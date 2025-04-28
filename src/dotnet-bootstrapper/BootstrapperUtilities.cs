using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Deployment.DotNet.Releases;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    internal static class BootstrapperUtilities
    {
        public static string GetRID()
        {
            string operatingSystem = RuntimeInformation.OSDescription switch
            {
                string os when os.Contains("Windows") => "win",
                string os when os.Contains("Linux") => "linux",
                string os when os.Contains("Darwin") => "osx",
                _ => null
            };
            string architecture = RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                _ => null
            };

            if (operatingSystem == null || architecture == null)
            {
                throw new PlatformNotSupportedException("Unsupported OS or architecture.");
            }

            return $"{operatingSystem}-{architecture}";
        }

        public static string GetMajorVersionToInstallInDirectory(string basePath)
        {
            // Get the nearest global.json file.
            JsonElement globalJson = GlobalJsonUtilities.GetNearestGlobalJson(basePath);
            string sdkVersion = globalJson
                .GetProperty("tools")
                .GetProperty("dotnet")
                .ToString();

            ReleaseVersion version = ReleaseVersion.Parse(sdkVersion);
            Console.WriteLine($"Found version {version.Major}.0 in global.json");
            return $"{version.Major}.0";
        }

        public static string GetInstallationDirectoryPath()
        {
            string globalJsonPath = GlobalJsonUtilities.GetNearestGlobalJsonPath(Environment.CurrentDirectory);
            if (globalJsonPath == null)
            {
                throw new FileNotFoundException("No global.json file found in the directory tree.");
            }
            string directoryPath = Path.GetDirectoryName(globalJsonPath);
            if (directoryPath == null)
            {
                throw new DirectoryNotFoundException("Directory path is null.");
            }

            // TODO: Replace with the actual installation directory.
            return Path.Combine(directoryPath, ".dotnet.local");
        }
    }
}
