using System;

namespace Microsoft.Build.Logging.Query.Scan
{
    public class ScanException : Exception
    {
        public ScanException(string expression) : base(expression)
        {
        }
    }
}