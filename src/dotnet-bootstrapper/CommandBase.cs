using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    public abstract class CommandBase
    {
        protected ParseResult _parseResult;

        protected CommandBase(ParseResult parseResult)
        {
            _parseResult = parseResult;
        }

        public abstract int Execute();
    }
}
