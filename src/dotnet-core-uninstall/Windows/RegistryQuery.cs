// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.MacOs;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;
using Microsoft.Win32;

namespace Microsoft.DotNet.Tools.Uninstall.Windows
{
    internal class RegistryQuery : IBundleCollector
    {
        public IEnumerable<Bundle> GetInstalledBundles()
        {
            return VisualStudioSafeVersionsExtractor.GetUninstallableBundles(GetAllInstalledBundles());
        }

        public virtual IEnumerable<Bundle> GetAllInstalledBundles()
        {
            var uninstalls = Registry.LocalMachine
                .OpenSubKey("SOFTWARE");

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 || RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                uninstalls = uninstalls.OpenSubKey("WOW6432Node");
            }

            uninstalls = uninstalls
                .OpenSubKey("Microsoft")
                .OpenSubKey("Windows")
                .OpenSubKey("CurrentVersion")
                .OpenSubKey("Uninstall");

            var names = uninstalls.GetSubKeyNames();

            var bundles = names
                .Select(name => uninstalls.OpenSubKey(name))
                .Where(bundle => IsDotNetCoreBundle(bundle));

            var wrappedBundles = bundles
              .Select(bundle => WrapRegistryKey(bundle))
              .Where(bundle => bundle != null)
              .ToList();

            return wrappedBundles;
        }

        private static bool IsDotNetCoreBundle(RegistryKey registryKey)
        {
            return IsDotNetCoreBundleDisplayName(registryKey.GetValue("DisplayName") as string)
                && IsDotNetCoreBundlePublisher(registryKey.GetValue("Publisher") as string)
                && IsDotNetCoreBundleUninstaller(registryKey.GetValue("WindowsInstaller") as int?)
                && IsNotVisualStudioDummyVersion(registryKey.GetValue("DisplayName") as string);
        }

        private static bool IsNotVisualStudioDummyVersion(string displayName)
        {
            return !displayName.Contains(" from Visual Studio");
        }

        private static bool IsDotNetCoreBundleDisplayName(string displayName)
        {
            return displayName == null ?
                false :
                Regexes.BundleDisplayNameRegex.IsMatch(displayName);
        }

        private static bool IsDotNetCoreBundlePublisher(string publisher)
        {
            return publisher == null ?
                false :
                Regexes.BundlePublisherRegex.IsMatch(publisher);
        }

        private static bool IsDotNetCoreBundleUninstaller(int? windowsInstaller)
        {
            return windowsInstaller == null;
        }

        private static Bundle WrapRegistryKey(RegistryKey registryKey)
        {
            var displayName = registryKey.GetValue("DisplayName") as string;
            var uninstallCommand = registryKey.GetValue("QuietUninstallString") as string;
            var bundleCachePath = registryKey.GetValue("BundleCachePath") as string;

            ParseVersionAndArch(registryKey, displayName, bundleCachePath, out var version, out var arch);

            if (version == null)
            {
                return null;
            }

            return Bundle.From(version, arch, uninstallCommand, displayName);
        }

        private static void ParseVersionAndArch(RegistryKey registryKey, string displayName, string bundleCachePath, out BundleVersion version, out BundleArch arch)
        {
            var match = Regexes.BundleDisplayNameRegex.Match(displayName);
            var cachePathMatch = Regexes.BundleCachePathRegex.Match(bundleCachePath);
            var archString = cachePathMatch.Groups[Regexes.ArchGroupName].Value ?? string.Empty;
            var versionFromCachePath = cachePathMatch.Groups[Regexes.VersionGroupName].Value;
            // Note: ASP.NET Core runtimes do not include version in the cache path, need to get version from registry:
            var versionFromRegistry = string.Join('.', (registryKey.GetValue("DisplayVersion") as string).Split('.').Take(3));
            var versionString = string.IsNullOrEmpty(versionFromCachePath) ? versionFromRegistry : versionFromCachePath;
            var hasAuxVersion = cachePathMatch.Groups[Regexes.AuxVersionGroupName].Success;
            var footnote = hasAuxVersion ?
                string.Format(LocalizableStrings.HostingBundleFootnoteFormat, displayName, versionString) :
                null;

            if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(versionString))
            {
                version = null;
                arch = BundleArch.X64 | BundleArch.X86;
                return;
            }

            switch (match.Groups[Regexes.TypeGroupName].Value)
            {
                case "SDK": version = new SdkVersion(versionString); break;
                case "Runtime": version = new RuntimeVersion(versionString); break;
                case "ASP.NET": version = new AspNetRuntimeVersion(versionString); break;
                case "Windows Server Hosting": version = new HostingBundleVersion(versionString, footnote); break;
                default: throw new ArgumentException();
            }

            switch (archString)
            {
                case "x64": arch = BundleArch.X64; break;
                case "x86": arch = BundleArch.X86; break;
                case "": arch = BundleArch.X64 | BundleArch.X86; break;
                default: throw new ArgumentException();
            }
        }

        public IEnumerable<BundleTypePrintInfo> GetSupportedBundleTypes()
        {
            return Windows.SupportedBundleTypeConfigs.SupportedBundleTypes;
        }
    }
}
