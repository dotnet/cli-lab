using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Messaging
{
    public class Error : Log
    {
        public Error(string text, BuildComponent parent) : base(text, parent)
        {
        }
    }
}
