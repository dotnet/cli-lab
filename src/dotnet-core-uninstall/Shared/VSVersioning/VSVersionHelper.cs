using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.DotNet.Tools.Uninstall.Properties;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.VisualStudio.Setup.Configuration;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    /// <summary>
    /// Extracts VS versions
    /// </summary>
    internal static class VSVersionHelper
    {
        private static List<SemanticVersion> Versions;

        private static Dictionary<SemanticVersion, SemanticVersion> VersionConversions;

        private const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);

        /// <summary>
        /// Converts visual studio version number to the .net core sdk version it depends on
        /// </summary>
        /// <returns>Null if the vs version doesn't depend on a dotnet version (vs2019 and later)</returns>
        public static SemanticVersion VSToDotnetVersion(SemanticVersion vsVersion)
        {
            if (VersionConversions == null)
            {
                // Deserialize static dictionary
                VersionConversions = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.VS_SDKVersions)
                    .Select((pair, i) => new KeyValuePair<SemanticVersion, SemanticVersion>(SemanticVersion.Parse(pair.Key), SemanticVersion.Parse(pair.Value)))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            if (!VersionConversions.TryGetValue(vsVersion, out SemanticVersion dotnetVersion))
            {
                // VS version doesn't depend on a dotnet distribution
                return null;
            }

            return dotnetVersion;
        }

        /// <summary>
        /// Returns all VS versions on the local machine
        /// </summary>
        public static IEnumerable<SemanticVersion> GetVSVersions()
        {
            if (Versions == null)
            {
                if (RuntimeInfo.RunningOnWindows)
                {
                    Versions = GetWindowsVersions();
                }
                else if (RuntimeInfo.RunningOnOSX)
                {
                    Versions = GetMacVersions();
                }
#if DEBUG
                Versions.Add(new SemanticVersion(0, 0, 0));
#endif
            }
            
            return Versions.AsReadOnly();
        }

        /// <summary>
        /// Returns all VS versions on the local windows machine
        /// </summary>
        /// <returns></returns>
        private static List<SemanticVersion> GetWindowsVersions()
        {
            var versions = new List<SemanticVersion>();
            try
            {
                var query = (ISetupConfiguration2)GenerateQuery();
                var e = query.EnumAllInstances();

                int fetched;
                var instances = new ISetupInstance[1];
                do
                {
                    e.Next(1, instances, out fetched);
                    if (fetched <= 0) continue;

                    var instance = instances[0];
                    var state = ((ISetupInstance2)instance).GetState();

                    if (!SemanticVersion.TryParse(instance.GetInstallationVersion(), out SemanticVersion version))
                        continue; // TODO better to fail silently-> might allow user to accidentally uninstall something important

                    versions.Add(version);
                } while (fetched > 0);
            }
            catch (COMException)
            {
            }
            catch (DllNotFoundException)
            {
            }
            return versions;
        }

        private static ISetupConfiguration GenerateQuery()
        {
            try
            {
                return new SetupConfiguration();
            }

            catch (COMException ex) when (ex.ErrorCode == REGDB_E_CLASSNOTREG)
            {
                // Try to get the class object using app-local call.
                var result = GetSetupConfiguration(out ISetupConfiguration query, IntPtr.Zero);

                if (result < 0)
                    throw new COMException($"Failed to get {nameof(query)}", result);

                return query;
            }
        }

        [DllImport("Microsoft.VisualStudio.Setup.Configuration.Native.dll", ExactSpelling = true, PreserveSig = true)]
        private static extern int GetSetupConfiguration(
            [MarshalAs(UnmanagedType.Interface)] [Out] out ISetupConfiguration configuration,
            IntPtr reserved);

        private static List<SemanticVersion> GetMacVersions()
        {
            return new List<SemanticVersion>(); // TODO
        }
    }
}
