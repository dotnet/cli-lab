using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Message
    {
        public string Text { get; }
        public MessageImportance Importance { get; }

        public Message(string text, MessageImportance importance)
        {
            Text = text;
            Importance = importance;
        }
    }
}
