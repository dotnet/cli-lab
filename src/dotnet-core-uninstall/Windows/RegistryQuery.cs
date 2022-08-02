// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var bundles = GetNetCoreBundleKeys(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64));
                bundles = bundles.Concat(GetNetCoreBundleKeys(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)));
                var wrappedBundles = new List<Bundle>();
                foreach(var bundle in bundles) {
                    if (WrapRegistryKey(bundle) is Bundle b) {
                        wrappedBundles.Add(b);
                    }
                }
                return wrappedBundles;
            }
            else
            {
                return Enumerable.Empty<Bundle>();
            }
        }

        [SupportedOSPlatform("windows")]
        private IEnumerable<RegistryKey> GetNetCoreBundleKeys(RegistryKey uninstallKey)
        {
            try
            {
                var uninstalls = uninstallKey
                    .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

                var names = uninstalls.GetSubKeyNames();
                return names
                    .Select(name => uninstalls.OpenSubKey(name))
                    .Where(bundle => IsNetCoreBundle(bundle));
            }
            catch
            {
                return Enumerable.Empty<RegistryKey>();
            }
        }

        [SupportedOSPlatform("windows")]

        private static bool IsNetCoreBundle(RegistryKey uninstallKey)
        {
            if (uninstallKey == null)
            {
                return false;
            }

            return IsNetCoreBundle(uninstallKey.GetValue("DisplayName") as string,
                uninstallKey.GetValue("DisplayVersion") as string,
                uninstallKey.GetValue("UninstallString") as string,
                uninstallKey.GetValue("BundleVersion") as string);
        }

        internal static bool IsNetCoreBundle(string displayName, string displayVersion, string uninstallString, string bundleVersion)
        {
            return (!String.IsNullOrEmpty(displayName)) &&
                    (displayName.IndexOf("Visual Studio", StringComparison.OrdinalIgnoreCase) < 0) &&
                    (displayName.IndexOf("VS 2015", StringComparison.OrdinalIgnoreCase) < 0) &&
                    (displayName.IndexOf("Local Feed", StringComparison.OrdinalIgnoreCase) < 0) &&
                    ((displayName.IndexOf(".NET", StringComparison.OrdinalIgnoreCase) >= 0) ||
                     (displayName.IndexOf(".NET Runtime", StringComparison.OrdinalIgnoreCase) >= 0) ||
                     (displayName.IndexOf(".NET SDK", StringComparison.OrdinalIgnoreCase) >= 0) ||
                     (displayName.IndexOf("Dotnet Shared Framework for Windows Desktop", StringComparison.OrdinalIgnoreCase) >= 0) ||
                     (displayName.IndexOf("Windows Desktop Runtime", StringComparison.OrdinalIgnoreCase) >= 0)) &&
                    (!String.IsNullOrEmpty(uninstallString)) &&
                    (uninstallString.IndexOf(".exe", StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (uninstallString.IndexOf("msiexec", StringComparison.OrdinalIgnoreCase) < 0) &&
                    (!String.IsNullOrEmpty(displayVersion)) &&
                    (!String.IsNullOrEmpty(bundleVersion));
        }

        [SupportedOSPlatform("windows")]
        private static Bundle WrapRegistryKey(RegistryKey registryKey)
        {
            var displayName = registryKey.GetValue("DisplayName") as string;
            var uninstallCommand = registryKey.GetValue("QuietUninstallString") as string ?? registryKey.GetValue("UninstallString") as string;
            var bundleCachePath = registryKey.GetValue("BundleCachePath") as string;

            var version = GetBundleVersion(displayName, uninstallCommand, bundleCachePath);
            var arch = GetBundleArch(displayName, bundleCachePath);

            if (version == null)
            {
                return null;
            }

            return Bundle.From(version, arch, uninstallCommand, displayName);
        }

        public static BundleVersion GetBundleVersion(string displayName, string uninstallString, string bundleCachePath)
        {
            var versionString = Regexes.VersionDisplayNameRegex.Match(displayName)?.Value ?? string.Empty;
            string footnote = null;
            if (bundleCachePath != null)
            {
                var cachePathMatch = Regexes.BundleCachePathRegex.Match(bundleCachePath);
                var hasAuxVersion = cachePathMatch.Groups[Regexes.AuxVersionGroupName].Success;
                footnote = hasAuxVersion ?
                    string.Format(LocalizableStrings.HostingBundleFootnoteFormat, displayName, versionString) :
                    null;
            }

            try
            {
                // Classify the bundle type
                if (displayName.IndexOf("Windows Server", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return new HostingBundleVersion(versionString, footnote);
                }
                else if (displayName.IndexOf("ASP.NET", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return new AspNetRuntimeVersion(versionString);
                }
                else if ((displayName.IndexOf(".NET Core SDK", StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (displayName.IndexOf("Microsoft .NET SDK", StringComparison.OrdinalIgnoreCase) >= 0) ||
                        uninstallString.IndexOf("dotnet-dev-win") >= 0)
                {
                    return new SdkVersion(versionString);
                }
                else if (displayName.IndexOf(".NET Core Runtime", StringComparison.OrdinalIgnoreCase) >= 0 || Regex.IsMatch(displayName, @".*\.NET Core.*Runtime") ||
                    displayName.IndexOf(".NET Runtime", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return new RuntimeVersion(versionString);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static BundleArch GetBundleArch(string displayName, string bundleCachePath)
        {
            const string x64String = "x64";
            const string x86String = "x86";
            const string arm64String = "arm64";

            string archString = null;
            if (bundleCachePath != null)
            {
                var cachePathMatch = Regexes.BundleCachePathRegex.Match(bundleCachePath);
                archString = cachePathMatch.Groups[Regexes.ArchGroupName].Value;
            }

            if (string.IsNullOrEmpty(archString))
            {
                archString = displayName.Contains(x64String) ?
                    x64String :
                    displayName.Contains(x86String) ? x86String : string.Empty;

                archString = archString switch
                {
                    string a when a.Contains(x64String) => x64String,
                    string b when b.Contains(x86String) => x86String,
                    string b when b.Contains(arm64String) => arm64String,
                    _ => string.Empty
                };
            }

            switch (archString)
            {
                case x64String: return BundleArch.X64;
                case x86String: return BundleArch.X86;
                case arm64String: return BundleArch.Arm64;
                case "": return BundleArch.X64 | BundleArch.X86;
                default: throw new ArgumentException();
            }
        }

        public IEnumerable<BundleTypePrintInfo> GetSupportedBundleTypes()
        {
            return Windows.SupportedBundleTypeConfigs.SupportedBundleTypes;
        }
    }
}
