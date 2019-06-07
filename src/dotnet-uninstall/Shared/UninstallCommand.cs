using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;

namespace Microsoft.DotNet.Tools.Uninstall.Shared
{
    internal static class UninstallCommand<TSdkInfo>
    {
        internal class OptionsConflictException : Exception { }

        private static readonly Dictionary<string, Filterer<TSdkInfo>> OptionFilterers
            = new Dictionary<string, Filterer<TSdkInfo>>()
            {
                { "--all", new Filterer<bool, TSdkInfo>((value, sdks) => sdks) }
            };

        private static readonly Filterer<IEnumerable<string>, TSdkInfo> NoOptionFilterer = new Filterer<IEnumerable<string>, TSdkInfo>((value, sdks) => sdks); // TODO: replace this

        internal static void AssertCorrectOptions(ParseResult parseResult)
        {
            var count = 0;

            foreach (var optionFiltererPair in OptionFilterers)
            {
                if (parseResult.HasOption(optionFiltererPair.Key))
                {
                    count++;
                }
            }

            if (count > 1)
            {
                throw new OptionsConflictException();
            }
        }

        internal static IEnumerable<TSdkInfo> SelectSdks(IEnumerable<TSdkInfo> sdks, ParseResult parseResult)
        {
            AssertCorrectOptions(parseResult);

            foreach (var optionFiltererPair in OptionFilterers)
            {
                if (parseResult.HasOption(optionFiltererPair.Key))
                {
                    return optionFiltererPair.Value.Filter(parseResult, optionFiltererPair.Key, sdks);
                }
            }

            return NoOptionFilterer.InternalFilterer(parseResult.RootCommandResult.Arguments, sdks);
        }
    }
}
