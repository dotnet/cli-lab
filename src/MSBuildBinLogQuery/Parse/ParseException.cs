using System;

namespace Microsoft.Build.Logging.Query.Parse
{
    public class ParseException : Exception
    {
        public ParseException(string expression) : base(expression)
        {
        }
    }
}