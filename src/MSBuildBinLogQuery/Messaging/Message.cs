using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Messaging
{
    public class Message : Log
    {
        public MessageImportance Importance { get; }

        public Message(string text, Component.Component parent, MessageImportance importance) : base(text, parent)
        {
            Importance = importance;
        }
    }
}