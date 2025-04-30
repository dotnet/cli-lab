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
    /// <summary>  
    /// Provides utility methods for handling .NET SDK installation and configuration.  
    /// </summary>  
    internal static class BootstrapperUtilities
    {
        /// <summary>  
        /// Retrieves the major and minor version of the .NET SDK specified in the nearest global.json file  
        /// within the given base path.  
        /// </summary>  
        /// <param name="basePath">The base directory path to search for the global.json file.</param>  
        /// <returns>  
        /// A string representing the major and minor version (e.g., "6.0") of the .NET SDK specified  
        /// in the global.json file.  
        /// </returns>  
        /// <exception cref="KeyNotFoundException">  
        /// Thrown when the required keys ("tools" or "dotnet") are not found in the global.json file.  
        /// </exception>  
        public static string GetMajorVersionToInstallInDirectory(string basePath)
        {
            try
            {
                // Get the nearest global.json file.  
                JsonElement globalJson = GlobalJsonUtilities.GetNearestGlobalJson(basePath);
                string sdkVersion = globalJson
                    .GetProperty("tools")
                    .GetProperty("dotnet")
                    .ToString();

                ReleaseVersion version = ReleaseVersion.Parse(sdkVersion);
                Console.WriteLine($"Found version {version.Major}.{version.Minor} in global.json");
                return $"{version.Major}.{version.Minor}";
            }
            catch (KeyNotFoundException e)
            {
                throw new KeyNotFoundException("The specified key was not found in the global.json", e);
            }
        }

        /// <summary>  
        /// Retrieves the installation directory path for the .NET SDK based on the nearest global.json file.  
        /// </summary>  
        /// <returns>  
        /// The full path to the installation directory, combining the directory of the global.json file  
        /// and a predefined subdirectory (".dotnet.local").  
        /// </returns>  
        /// <exception cref="FileNotFoundException">  
        /// Thrown when no global.json file is found in the directory tree.  
        /// </exception>  
        /// <exception cref="DirectoryNotFoundException">  
        /// Thrown when the directory path derived from the global.json file is null.  
        /// </exception>  
        public static string GetInstallationDirectoryPath()
        {
            string globalJsonPath = GlobalJsonUtilities.GetNearestGlobalJsonPath(Environment.CurrentDirectory);
            if (globalJsonPath == null)
            {
                throw new FileNotFoundException($"No global.json file found in the directory tree.", Environment.CurrentDirectory);
            }
            string directoryPath = Path.GetDirectoryName(globalJsonPath);
            // TODO: Replace with the actual installation directory.  
            return Path.Combine(directoryPath, ".dotnet.local");
        }
    }
}
