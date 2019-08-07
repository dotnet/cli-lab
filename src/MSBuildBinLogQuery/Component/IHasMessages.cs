using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Component
{
    public interface ILoggable
    {
        IList<Message> Messages { get; }
    }
}