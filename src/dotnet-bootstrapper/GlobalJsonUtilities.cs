using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Microsoft.Deployment.DotNet.Releases;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    /// <summary>
    /// Provides utility methods for working with global.json files in a directory tree.
    /// </summary>
    internal static class GlobalJsonUtilities
    {
        /// <summary>
        /// Finds the nearest global.json file path by traversing up the directory tree starting from the specified base path.
        /// </summary>
        /// <param name="basePath">The starting directory path to search for the global.json file.</param>
        /// <returns>
        /// The full path to the nearest global.json file if found; otherwise, null.
        /// </returns>
        public static string GetNearestGlobalJsonPath(string basePath)
        {
            // Example implementation: Traverse up the directory tree to find the nearest global.json file.
            string currentPath = basePath;
            while (!string.IsNullOrEmpty(currentPath))
            {
                string globalJsonPath = Path.Combine(currentPath, "global.json");
                if (File.Exists(globalJsonPath))
                {
                    return globalJsonPath;
                }
                currentPath = Directory.GetParent(currentPath)?.FullName;
            }
            // If no global.json file is found, return null.
            return null;
        }

        /// <summary>
        /// Retrieves the contents of the nearest global.json file as a JsonElement by traversing up the directory tree.
        /// </summary>
        /// <param name="basePath">The starting directory path to search for the global.json file.</param>
        /// <returns>
        /// A JsonElement representing the contents of the nearest global.json file.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if no global.json file is found in the directory tree.</exception>
        /// <exception cref="JsonException">Thrown if global.json cannot be parsed as json</exception>
        public static JsonElement GetNearestGlobalJson(string basePath)
        {
            string globalJsonPath = GetNearestGlobalJsonPath(basePath);

            if (globalJsonPath == null)
            {
                throw new FileNotFoundException("No global.json file found in the directory tree.");
            }

            return JsonDocument.Parse(File.ReadAllText(globalJsonPath)).RootElement;
        }
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
    }
}
