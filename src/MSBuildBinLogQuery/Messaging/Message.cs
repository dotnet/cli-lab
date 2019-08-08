using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Messaging
{
    public class Message : Log
    {
        public MessageImportance Importance { get; }

        public Message(string text, BuildComponent parent, MessageImportance importance) : base(text, parent)
        {
            Importance = importance;
        }
    }
}
