using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal abstract class Filterer
    {
        public abstract bool AcceptMultipleBundleTypes { get; }

        public abstract IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection);

        protected void ValidateTypeSelection(BundleType typeSelection)
        {
            if ((int) typeSelection < 1 || typeSelection > (BundleType.Sdk | BundleType.Runtime))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!AcceptMultipleBundleTypes && typeSelection != BundleType.Sdk && typeSelection != BundleType.Runtime)
            {
                throw new BundleTypeNotSpecifiedException();
            }
        }
    }

    internal abstract class ArgFilterer<TArg> : Filterer
    {
        public abstract override bool AcceptMultipleBundleTypes { get; }

        public override IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            var argValue = parseResult.ValueForOption<TArg>(option.Name);
            return Filter(argValue, bundles, typeSelection);
        }

        public IEnumerable<Bundle> Filter(TArg argValue, IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            ValidateTypeSelection(typeSelection);

            var sdks = Bundle<SdkVersion>.FilterWithSameBundleType(bundles);
            var runtimes = Bundle<RuntimeVersion>.FilterWithSameBundleType(bundles);

            return ((typeSelection & BundleType.Sdk) > 0 ? Filter(argValue, sdks).Select(sdk => sdk as Bundle) : new List<Bundle>())
                .Concat((typeSelection & BundleType.Runtime) > 0 ? Filter(argValue, runtimes).Select(runtime => runtime as Bundle) : new List<Bundle>());
        }

        public abstract IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(TArg argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>;
    }

    internal abstract class NoArgFilterer : Filterer
    {
        public abstract override bool AcceptMultipleBundleTypes { get; }

        public override IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            return Filter(bundles, typeSelection);
        }

        public IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            ValidateTypeSelection(typeSelection);

            var sdks = Bundle<SdkVersion>.FilterWithSameBundleType(bundles);
            var runtimes = Bundle<RuntimeVersion>.FilterWithSameBundleType(bundles);

            return ((typeSelection & BundleType.Sdk) > 0 ? Filter(sdks).Select(sdk => sdk as Bundle) : new List<Bundle>())
                .Concat((typeSelection & BundleType.Runtime) > 0 ? Filter(runtimes).Select(runtime => runtime as Bundle) : new List<Bundle>());
        }

        public abstract IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<Bundle<TBundleVersion>> bundles)
            where TBundleVersion : BundleVersion, IComparable<TBundleVersion>;
    }
}
