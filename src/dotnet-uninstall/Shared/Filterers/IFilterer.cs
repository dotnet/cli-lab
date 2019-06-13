using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal interface IFilterer
    {
        IEnumerable<BundleInfo.Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<BundleInfo.Bundle> bundles);
    }

    internal abstract class ArgFilterer<TArg> : IFilterer
    {
        public IEnumerable<BundleInfo.Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<BundleInfo.Bundle> bundles)
        {
            return Filter(parseResult.ValueForOption<TArg>(option.Name), bundles);
        }

        public abstract IEnumerable<BundleInfo.Bundle> Filter(TArg argValue, IEnumerable<BundleInfo.Bundle> bundles);
    }

    internal abstract class NoArgFilterer : IFilterer
    {
        public IEnumerable<BundleInfo.Bundle> Filter(ParseResult parseResult, Option option, IEnumerable<BundleInfo.Bundle> bundles)
        {
            return Filter(bundles);
        }

        public abstract IEnumerable<BundleInfo.Bundle> Filter(IEnumerable<BundleInfo.Bundle> bundles);
    }
}
