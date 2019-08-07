using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Component
{
    public interface IHasMessages
    {
        IList<Message> Messages { get; }
    }
}