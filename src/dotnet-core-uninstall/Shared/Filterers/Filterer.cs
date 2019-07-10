using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal abstract class Filterer
    {
        public abstract IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection, BundleArch archSelection);

        protected void ValidateTypeSelection(BundleType typeSelection)
        {
            if ((int) typeSelection < 1 || typeSelection > (BundleType.Sdk | BundleType.Runtime))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (typeSelection != BundleType.Sdk && typeSelection != BundleType.Runtime)
            {
                throw new BundleTypeMissingException();
            }
        }

        protected void ValidateArchSelection(BundleArch archSelection)
        {
            if ((int)archSelection < 1 || archSelection > (BundleArch.X86 | BundleArch.X64))
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal abstract class ArgFilterer<TArg> : Filterer
    {
        public override IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection, BundleArch archSelection)
        {
            var argValue = parseResult.ValueForOption<TArg>(option.Name);
            return Filter(argValue, bundles, typeSelection, archSelection);
        }

        public IEnumerable<Bundle> Filter(TArg argValue, IEnumerable<Bundle> bundles, BundleType typeSelection, BundleArch archSelection)
        {
            ValidateTypeSelection(typeSelection);
            ValidateArchSelection(archSelection);

            var filteredBundlesByArch = bundles.Where(bundle => archSelection.HasFlag(bundle.Arch));

            var sdks = Bundle<SdkVersion>.FilterWithSameBundleType(filteredBundlesByArch);
            var runtimes = Bundle<RuntimeVersion>.FilterWithSameBundleType(filteredBundlesByArch);

            var filteredSdks = typeSelection.HasFlag(BundleType.Sdk) ?
                Filter(argValue, sdks).OrderBy(sdk => sdk).Select(sdk => sdk as Bundle) :
                new List<Bundle>();

            var filteredRuntimes = typeSelection.HasFlag(BundleType.Runtime) ?
                Filter(argValue, runtimes).OrderBy(runtime => runtime).Select(runtime => runtime as Bundle) :
                new List<Bundle>();

            return filteredSdks.Concat(filteredRuntimes);
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

            var filteredSdks = typeSelection.HasFlag(BundleType.Sdk) ?
                Filter(sdks).OrderBy(sdk => sdk).Select(sdk => sdk as Bundle) :
                new List<Bundle>();

            var filteredRuntimes = typeSelection.HasFlag(BundleType.Runtime) ?
                Filter(runtimes).OrderBy(runtime => runtime).Select(runtime => runtime as Bundle) :
                new List<Bundle>();

            return filteredSdks.Concat(filteredRuntimes);
        }

        public abstract IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<Bundle<TBundleVersion>> bundles)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>;
    }
}
