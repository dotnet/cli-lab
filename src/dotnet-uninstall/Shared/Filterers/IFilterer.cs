using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal interface IFilterer
    {
        IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection);
    }

    internal abstract class ArgFilterer<TArg> : IFilterer
    {
        public IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            return Filter(parseResult.ValueForOption<TArg>(option.Name), bundles, typeSelection);
        }

        public abstract IEnumerable<Bundle> Filter(TArg argValue, IEnumerable<Bundle> bundles, BundleType typeSelection);
    }

    internal abstract class NoArgFilterer : IFilterer
    {
        public IEnumerable<Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<Bundle> bundles, BundleType typeSelection)
        {
            return Filter(bundles, typeSelection);
        }

        public abstract IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles, BundleType typeSelection);
    }
}
