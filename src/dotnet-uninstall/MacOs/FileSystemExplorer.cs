using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.MacOs
{
    internal static class FileSystemExplorer
    {
        private static readonly string DotNetInstallPath = Path.Combine("/", "usr", "local", "share", "dotnet");
        private static readonly string DotNetSdkInstallPath = Path.Combine(DotNetInstallPath, "sdk");
        private static readonly string DotNetRuntimeInstallPath = Path.Combine(DotNetInstallPath, "shared", "Microsoft.NETCore.App");

        public static IEnumerable<Bundle> GetInstalledBundles()
        {
            var sdks = GetInstalledBundles<SdkVersion>(DotNetSdkInstallPath);
            var runtimes = GetInstalledBundles<RuntimeVersion>(DotNetRuntimeInstallPath);

            return sdks.Concat(runtimes);
        }

        private static IEnumerable<Bundle> GetInstalledBundles<TBundleVersion>(string path)
            where TBundleVersion : BundleVersion, new()
        {
            return new DirectoryInfo(path)
                .EnumerateDirectories()
                .Select(dirInfo =>
                {
                    var success = BundleVersion.TryFromInput<TBundleVersion>(dirInfo.Name, out var version);
                    return (success, version, path: dirInfo.FullName);
                })
                .Where(tuple => tuple.success)
                .Select(tuple => Bundle<SdkVersion>.From(tuple.version, BundleArch.X64, GetUninstallCommand(tuple.path)));
        }

        private static string GetUninstallCommand(string path)
        {
            return $"sudo rm -rf {path}";
        }
    }
}
