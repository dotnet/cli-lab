using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        public static string GetVersionToInstallInDirectory(string directoryPath)
        {
            // Add heuristic to determine the version to install based on project, environment, etc.
            // For now, return a hardcoded version.
            return "9.0";
        }
        public static string GetInstallationDirectoryPath()
        {
            // Add heuristic to determine the installation directory based on project, environment, etc.
            return Path.Join(
                Environment.CurrentDirectory,
                ".dotnet.test");
        }
    }
}
