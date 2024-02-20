﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal abstract class Filterer
    {
        public abstract IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection, BundleArch archSelection);

        protected void ValidateTypeSelection(BundleType typeSelection)
        {
            var bundleTypes = Enum.GetValues(typeof(BundleType)).OfType<BundleType>();

            if ((int) typeSelection < 1 || typeSelection > bundleTypes.Aggregate((BundleType)0, (orSum, next) => orSum | next))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!bundleTypes.Contains(typeSelection))
            {
                throw new BundleTypeMissingException(SupportedBundleTypeConfigs
                    .GetSupportedBundleTypes()
                    .OrderBy(type => type.OptionName)
                    .Select(type => $"--{type.OptionName}"));
            }
        }

        protected void ValidateArchSelection(BundleArch archSelection)
        {
            if ((int)archSelection < 1 || archSelection > Enum.GetValues(typeof(BundleArch)).OfType<BundleArch>().Aggregate((BundleArch)0, (orSum, next) => orSum | next))
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal abstract class ArgFilterer<TArg> : Filterer
    {
        public override IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection, BundleArch archSelection)
        {
            var argValue = parseResult.GetValueForOption<TArg>(option as Option<TArg>);
            return Filter(argValue, bundles, typeSelection, archSelection);
        }

        public IEnumerable<Bundle> Filter(TArg argValue, IEnumerable<Bundle> bundles, BundleType typeSelection, BundleArch archSelection)
        {
            ValidateTypeSelection(typeSelection);
            ValidateArchSelection(archSelection);

            var filteredBundlesByArch = bundles.Where(bundle => archSelection.HasFlag(bundle.Arch));

            var sdks = Bundle<SdkVersion>.FilterWithSameBundleType(filteredBundlesByArch);
            var runtimes = Bundle<RuntimeVersion>.FilterWithSameBundleType(filteredBundlesByArch);
            var aspNetRuntimes = Bundle<AspNetRuntimeVersion>.FilterWithSameBundleType(filteredBundlesByArch);
            var hostingBundles = Bundle<HostingBundleVersion>.FilterWithSameBundleType(filteredBundlesByArch);

            var filteredSdks = typeSelection.HasFlag(BundleType.Sdk) ?
                Filter(argValue, sdks).OrderBy(sdk => sdk).Select(sdk => sdk as Bundle) :
                new List<Bundle>();

            var filteredRuntimes = typeSelection.HasFlag(BundleType.Runtime) ?
                Filter(argValue, runtimes).OrderBy(runtime => runtime).Select(runtime => runtime as Bundle) :
                new List<Bundle>();

            var filteredAspNetRuntimes = typeSelection.HasFlag(BundleType.AspNetRuntime) ?
                Filter(argValue, aspNetRuntimes).OrderBy(aspNetRuntime => aspNetRuntime).Select(aspNetRuntime => aspNetRuntime as Bundle) :
                new List<Bundle>();

            var filteredHostingBundles = typeSelection.HasFlag(BundleType.HostingBundle) ?
                Filter(argValue, hostingBundles).OrderBy(hostingBundle => hostingBundle).Select(hostingBundle => hostingBundle as Bundle) :
                new List<Bundle>();

            return filteredSdks
                .Concat(filteredRuntimes)
                .Concat(filteredAspNetRuntimes)
                .Concat(filteredHostingBundles);
        }

        public abstract IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(TArg argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>, new();
    }

    internal abstract class NoArgFilterer : Filterer
    {
        public override IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection, BundleArch archSelection)
        {
            return Filter(bundles, typeSelection, archSelection);
        }

        public IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles, BundleType typeSelection, BundleArch archSelection)
        {
            ValidateTypeSelection(typeSelection);
            ValidateArchSelection(archSelection);

            var filteredBundlesByArch = bundles.Where(bundle => archSelection.HasFlag(bundle.Arch));

            var sdks = Bundle<SdkVersion>.FilterWithSameBundleType(filteredBundlesByArch);
            var runtimes = Bundle<RuntimeVersion>.FilterWithSameBundleType(filteredBundlesByArch);
            var aspNetRuntimes = Bundle<AspNetRuntimeVersion>.FilterWithSameBundleType(filteredBundlesByArch);
            var hostingBundles = Bundle<HostingBundleVersion>.FilterWithSameBundleType(filteredBundlesByArch);

            var filteredSdks = typeSelection.HasFlag(BundleType.Sdk) ?
                Filter(sdks).OrderBy(sdk => sdk).Select(sdk => sdk as Bundle) :
                new List<Bundle>();

            var filteredRuntimes = typeSelection.HasFlag(BundleType.Runtime) ?
                Filter(runtimes).OrderBy(runtime => runtime).Select(runtime => runtime as Bundle) :
                new List<Bundle>();

            var filteredAspNetRuntimes = typeSelection.HasFlag(BundleType.AspNetRuntime) ?
                Filter(aspNetRuntimes).OrderBy(aspNetRuntime => aspNetRuntime).Select(aspNetRuntime => aspNetRuntime as Bundle) :
                new List<Bundle>();

            var filteredHostingBundles = typeSelection.HasFlag(BundleType.HostingBundle) ?
                Filter(hostingBundles).OrderBy(hostingBundle => hostingBundle).Select(hostingBundle => hostingBundle as Bundle) :
                new List<Bundle>();

            return filteredSdks
                .Concat(filteredRuntimes)
                .Concat(filteredAspNetRuntimes)
                .Concat(filteredHostingBundles);
        }

        public abstract IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<Bundle<TBundleVersion>> bundles)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>;
    }
}
