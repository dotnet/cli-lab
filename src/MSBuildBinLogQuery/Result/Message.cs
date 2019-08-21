using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Result
{
    public class Message : Log
    {
        public MessageImportance Importance { get; }

        public Message(string text, Component parent, MessageImportance importance) : base(text, parent)
        {
            Importance = importance;
        }
    }
}