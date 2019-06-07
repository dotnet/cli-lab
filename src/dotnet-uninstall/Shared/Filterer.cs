using System;
using System.Collections.Generic;
using System.CommandLine;

namespace Microsoft.DotNet.Tools.Uninstall.Shared
{
    internal abstract class Filterer<TSdkInfo>
    {
        internal abstract IEnumerable<TSdkInfo> Filter(ParseResult parseResult, string option, IEnumerable<TSdkInfo> list);
    }

    internal class Filterer<TOption, TSdkInfo> : Filterer<TSdkInfo>
    {
        internal Func<TOption, IEnumerable<TSdkInfo>, IEnumerable<TSdkInfo>> InternalFilterer { get; }

        internal Filterer(Func<TOption, IEnumerable<TSdkInfo>, IEnumerable<TSdkInfo>> filterer)
        {
            InternalFilterer = filterer;
        }

        internal override IEnumerable<TSdkInfo> Filter(ParseResult parseResult, string option, IEnumerable<TSdkInfo> list)
        {
            return InternalFilterer(parseResult.ValueForOption<TOption>(option), list);
        }
    }
}
