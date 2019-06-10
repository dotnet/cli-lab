using System.Collections.Generic;
using System.CommandLine;
using Microsoft.DotNet.Tools.Uninstall.Shared.SdkInfo;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal interface IFilterer
    {
        IEnumerable<ISdkInfo> Filter(ParseResult parseResult, Option option, IEnumerable<ISdkInfo> sdks);
    }

    internal abstract class ArgFilterer<TArg> : IFilterer
    {
        public IEnumerable<ISdkInfo> Filter(ParseResult parseResult, Option option, IEnumerable<ISdkInfo> sdks)
        {
            return Filter(parseResult.ValueForOption<TArg>(option.Name), sdks);
        }

        public abstract IEnumerable<ISdkInfo> Filter(TArg argValue, IEnumerable<ISdkInfo> sdks);
    }

    internal abstract class NoArgFilterer : IFilterer
    {
        public IEnumerable<ISdkInfo> Filter(ParseResult parseResult, Option option, IEnumerable<ISdkInfo> sdks)
        {
            return Filter(sdks);
        }

        public abstract IEnumerable<ISdkInfo> Filter(IEnumerable<ISdkInfo> sdks);
    }
}
