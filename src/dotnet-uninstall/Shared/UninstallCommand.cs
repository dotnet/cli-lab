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
                { "--all", new Filterer<bool, TSdkInfo>((value, sdks) => sdks) },
                { "--all-lower-patches", new Filterer<bool, TSdkInfo>((value, sdks) => sdks) },       // TODO: stub
                { "--all-but-latest", new Filterer<bool, TSdkInfo>((value, sdks) => sdks) },          // TODO: stub
                { "--all-but", new Filterer<IEnumerable<string>, TSdkInfo>((value, sdks) => sdks) },  // TODO: stub
                { "--all-below", new Filterer<string, TSdkInfo>((value, sdks) => sdks) },             // TODO: stub
                { "--all-previews", new Filterer<bool, TSdkInfo>((value, sdks) => sdks) },            // TODO: stub
                { "--all-previews-but-latest", new Filterer<bool, TSdkInfo>((value, sdks) => sdks) }, // TODO: stub
                { "--major-minor", new Filterer<string, TSdkInfo>((value, sdks) => sdks) }            // TODO: stub
            };

        private static readonly Filterer<IEnumerable<string>, TSdkInfo> NoOptionFilterer
            = new Filterer<IEnumerable<string>, TSdkInfo>((value, sdks) => sdks); // TODO: stub

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

            if (count > 1 || count == 1 && parseResult.RootCommandResult.Arguments.Count > 0)
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
