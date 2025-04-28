using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    internal static class GlobalJsonUtilities
    {
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

        public static JsonElement GetNearestGlobalJson(string basePath)
        {
            string globalJsonPath = GetNearestGlobalJsonPath(basePath);

            if (globalJsonPath == null)
            {
                throw new FileNotFoundException("No global.json file found in the directory tree.");
            }

            return JsonDocument.Parse(File.ReadAllText(globalJsonPath)).RootElement;
        }
    }
}
