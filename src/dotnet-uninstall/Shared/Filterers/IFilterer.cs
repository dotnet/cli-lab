using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal interface IFilterer
    {
        IEnumerable<IBundleInfo> Filter(ParseResult parseResult, Option option, IEnumerable<IBundleInfo> bundles);
    }

    internal abstract class ArgFilterer<TArg> : IFilterer
    {
        public IEnumerable<IBundleInfo> Filter(ParseResult parseResult, Option option, IEnumerable<IBundleInfo> bundles)
        {
            return Filter(parseResult.ValueForOption<TArg>(option.Name), bundles);
        }

        public abstract IEnumerable<IBundleInfo> Filter(TArg argValue, IEnumerable<IBundleInfo> bundles);
    }

    internal abstract class NoArgFilterer : IFilterer
    {
        public IEnumerable<IBundleInfo> Filter(ParseResult parseResult, Option option, IEnumerable<IBundleInfo> bundles)
        {
            return Filter(bundles);
        }

        public abstract IEnumerable<IBundleInfo> Filter(IEnumerable<IBundleInfo> bundles);
    }
}
