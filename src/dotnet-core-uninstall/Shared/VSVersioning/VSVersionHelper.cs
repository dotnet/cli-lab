using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.Properties;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.VisualStudio.Setup.Configuration;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning
{
    /// <summary>
    /// Extracts VS versions and determine which sdk's are safe to uninstall
    /// </summary>
    internal static class VSVersionHelper
    {
        private static Dictionary<SemanticVersion, SemanticVersion> VersionConversions;

        private const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);

        /// <summary>
        /// Assigns the uninstallAllowed flag for each sdkVersion in sdkVersions
        /// </summary>
        public static IEnumerable<Bundle> AssignUninstallAllowed(IEnumerable<Bundle> bundles)
        {
            // TODO add filtering by provider key?
            var InstalledSdkBundles = bundles
                .Where(b => b.Version is SdkVersion)
                .Select((b, i) => b as Bundle<SdkVersion>);
            IEnumerable<SemanticVersion> VSVersions = GetVSVersions();
            // For each version of vs, mark the .net core sdk with the highest version number as uninstallable
            foreach (SemanticVersion vsVersion in VSVersions)
            {
                var sdkDepends = VSToDotnetVersionRange(vsVersion);
                if (sdkDepends != null)
                {
                    var intersection = InstalledSdkBundles
                        .Where(bundle => (sdkDepends.Item1 <= bundle.Version.SemVer && bundle.Version.SemVer < sdkDepends.Item2)); // Filter for valid sdks
                    if (intersection.Count() > 0)
                    {
                        bundles.First(b => b.Equals(intersection.Max())).UninstallAllowed = false;
                    }
                }
            }
            return bundles;
        }

        /// <summary>
        /// Converts visual studio version to tuple of [the minimum .net core sdk version, the max .net core version it depends on)
        /// </summary>
        /// <returns>null if the vs version doesn't depend on a dotnet version</returns>
        private static Tuple<SemanticVersion, SemanticVersion> VSToDotnetVersionRange(SemanticVersion vsVersion)
        {
            if ((vsVersion.Major < 15 || vsVersion.Major >= 16)
#if DEBUG
                && vsVersion.Major > 0 // Include mock data when debugging
#endif
                )
            {
                // VS version doesn't depend on a dotnet distribution
                return null;
            }
            if (VersionConversions == null)
            {
                // Deserialize static dictionary
                VersionConversions = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.VS_SDKVersions)
                    .Select((pair, i) => new KeyValuePair<SemanticVersion, SemanticVersion>(
                        SemanticVersion.Parse(pair.Key), SemanticVersion.Parse(pair.Value)))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            if (!VersionConversions.TryGetValue(vsVersion, out SemanticVersion dotnetVersion)) 
            {
                // If we don't have the exact vs version in the map, choose the closest // TODO right approach?-> have all major.minor in map?
                dotnetVersion = VersionConversions
                    .Where(item => vsVersion.Major == item.Key.Major && vsVersion.Minor == item.Key.Minor)
                    .OrderBy(item => Math.Abs(vsVersion.Patch - item.Key.Patch)).First().Value; 
            }
            return new Tuple<SemanticVersion, SemanticVersion>(dotnetVersion, new SemanticVersion(dotnetVersion.Major, dotnetVersion.Minor + 1, 0)); 
        }

        /// <summary>
        /// Check that the sdkbundles in bundles are uninstallable, throw an error if not
        /// </summary>
        public static void CheckUninstallable(IEnumerable<Bundle> bundles)
        {
            var uninstallablebundles = bundles
                .Where(item => (item.Version is SdkVersion) && !item.UninstallAllowed);
            if (uninstallablebundles.Count() > 0)
            {
                // Invalid command, trying to uninstall a sdk vs relies on-> fail
                throw new UninstallationNotAllowedException(GetErrorStringList(uninstallablebundles));
            }
        }

        /// <summary>
        /// Returns an error string listing uninstallable sdk versions
        /// </summary>
        public static string GetErrorStringList(IEnumerable<Bundle> bundles)
        {
            var errorString = string.Empty;
            foreach(Bundle b in bundles)
            {
                errorString += "\n\t" + b.DisplayName;
            }
            
            return errorString;
        }

        /// <summary>
        /// Returns all VS versions on the local windows machine
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<SemanticVersion> GetVSVersions()
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

                    var installedVersion = instance.GetInstallationVersion(); // TODO this returns a different patch num format than listed in jsons
                    if (!Version.TryParse(installedVersion, out Version version))
                        continue;

                    versions.Add(new SemanticVersion(version.Major, version.Minor, version.Revision));
                } while (fetched > 0);
            }
            catch (COMException)
            {
            }
            catch (DllNotFoundException)
            {
            }
#if DEBUG
            versions.Add(new SemanticVersion(0, 0, 0));
            versions.Add(new SemanticVersion(0, 0, 1));
            versions.Add(new SemanticVersion(0, 0, 2));
#endif
            return versions.AsReadOnly();
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
    }
}
