using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Messaging
{
    public class Message : Log
    {
        public MessageImportance Importance { get; }

        public Message(string text, MessageImportance importance) : base(text)
        {
            Importance = importance;
        }
    }
}
