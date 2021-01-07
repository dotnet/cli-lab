// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;

namespace Microsoft.DotNet.Tools.Uninstall.MacOs
{
    internal class FileSystemExplorer : IBundleCollector
    {
        private static readonly string DotNetInstallPath = Path.Combine("/", "usr", "local", "share", "dotnet");
        private static readonly string DotNetSdkInstallPath = Path.Combine(DotNetInstallPath, "sdk");
        private static readonly string DotNetRuntimeInstallPath = Path.Combine(DotNetInstallPath, "shared", "Microsoft.NETCore.App");
        private static readonly string DotNetAspAllInstallPath = Path.Combine(DotNetInstallPath, "shared", "Microsoft.AspNetCore.All");
        private static readonly string DotNetAspAppInstallPath = Path.Combine(DotNetInstallPath, "shared", "Microsoft.AspNetCore.App");
        private static readonly string DotNetHostFxrInstallPath = Path.Combine(DotNetInstallPath, "host", "fxr");

        public virtual IEnumerable<Bundle> GetAllInstalledBundles()
        {
            var sdks = GetInstalledBundles<SdkVersion>(DotNetSdkInstallPath);
            var runtimes = GetInstalledBundles<RuntimeVersion>(
                DotNetRuntimeInstallPath,
                DotNetAspAllInstallPath,
                DotNetAspAppInstallPath,
                DotNetHostFxrInstallPath);

            return sdks.Concat(runtimes).ToList();
        }

        private static IEnumerable<Bundle> GetInstalledBundles<TBundleVersion>(params string[] paths)
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
                    BundleArch.X64,
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
