// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;

namespace Microsoft.DotNet.Tools.Uninstall.MacOs
{
    internal class FileSystemExplorer : IBundleCollector
    {
        private static readonly string DotNetInstallPath = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_INSTALL_DIR")) ?
            Path.Combine("/", "usr", "local", "share", "dotnet") :
            Environment.GetEnvironmentVariable("DOTNET_INSTALL_DIR");
        private static readonly string EmulatedDotNetInstallPath = Path.Combine(DotNetInstallPath, "x64");
        private static string DotNetSdkInstallPath(string dotnetRootPath) => Path.Combine(dotnetRootPath, "sdk");
        private static string DotNetRuntimeInstallPath(string dotnetRootPath) => Path.Combine(dotnetRootPath, "shared", "Microsoft.NETCore.App");
        private static string DotNetAspAllInstallPath(string dotnetRootPath) => Path.Combine(dotnetRootPath, "shared", "Microsoft.AspNetCore.All");
        private static  string DotNetAspAppInstallPath(string dotnetRootPath) => Path.Combine(dotnetRootPath, "shared", "Microsoft.AspNetCore.App");
        private static string DotNetHostFxrInstallPath(string dotnetRootPath) => Path.Combine(dotnetRootPath, "host", "fxr");

        public virtual IEnumerable<Bundle> GetAllInstalledBundles()
        {
            var nativeArch = IsMacx64Installation(DotNetInstallPath) ? BundleArch.X64 : BundleArch.Arm64;
            var sdks = GetInstalledBundles<SdkVersion>(nativeArch, DotNetSdkInstallPath(DotNetInstallPath));
            var runtimes = GetInstalledBundles<RuntimeVersion>(
                nativeArch,
                DotNetRuntimeInstallPath(DotNetInstallPath),
                DotNetAspAllInstallPath(DotNetInstallPath),
                DotNetAspAppInstallPath(DotNetInstallPath),
                DotNetHostFxrInstallPath(DotNetInstallPath));

            if (Directory.Exists(EmulatedDotNetInstallPath))
            {
                sdks = sdks.Concat(GetInstalledBundles<SdkVersion>(BundleArch.X64, DotNetSdkInstallPath(EmulatedDotNetInstallPath)));
                runtimes = runtimes.Concat(GetInstalledBundles<RuntimeVersion>(
                    BundleArch.X64,
                    DotNetRuntimeInstallPath(DotNetInstallPath),
                    DotNetAspAllInstallPath(DotNetInstallPath),
                    DotNetAspAppInstallPath(DotNetInstallPath),
                    DotNetHostFxrInstallPath(DotNetInstallPath)));
            }

            return sdks.Concat(runtimes).ToList();
        }

        private static bool IsMacx64Installation(string path)
        {
            try
            {
                var versionDirs = Directory.GetDirectories(Path.Combine(path, "sdk"));
                var rids = File.ReadAllText(Path.Combine(versionDirs[0], "NETCoreSdkRuntimeIdentifierChain.txt"));
                return !rids.Contains("arm64");
            }
            catch
            {
                return true;
            }
        }

        private static IEnumerable<Bundle> GetInstalledBundles<TBundleVersion>(BundleArch arch, params string[] paths)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>, new()
        {
            string bundleTypeString;
            switch (new TBundleVersion().Type)
            {
                case BundleType.Sdk: bundleTypeString = "SDK"; break;
                case BundleType.Runtime: bundleTypeString = "Runtime"; break;
                default: throw new ArgumentException();
            }

            return paths
                .SelectMany(path => GetInstalledVersionsAndUninstallCommands<TBundleVersion>(path))
                .GroupBy(tuple => tuple.Version)
                .Select(group => Bundle.From(
                    group.First().Version,
                    arch,
                    GetUninstallCommand(group.Select(tuple => tuple.Path)),
                    string.Format(LocalizableStrings.MacOsBundleDisplayNameFormat, bundleTypeString, group.First().Version.ToString())));
        }

        private static IEnumerable<(TBundleVersion Version, string Path)> GetInstalledVersionsAndUninstallCommands<TBundleVersion>(string path)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>, new()
        {
            return Directory.Exists(path) ?
                new DirectoryInfo(path)
                    .EnumerateDirectories()
                    .Select(dirInfo =>
                    {
                        var success = BundleVersion.TryFromInput<TBundleVersion>(dirInfo.Name, out var version);
                        return (Success: success, Version: version, Path: dirInfo.FullName);
                    })
                    .Where(tuple => tuple.Success)
                    .Select(tuple => (tuple.Version, tuple.Path)) :
                new List<(TBundleVersion Version, string Path)>();
        }

        private static string GetUninstallCommand(IEnumerable<string> paths)
        {
            return string.Join(" ", paths);
        }

        public IEnumerable<BundleTypePrintInfo> GetSupportedBundleTypes()
        {
            return MacOs.SupportedBundleTypeConfigs.SupportedBundleTypes;
        }
    }
}
