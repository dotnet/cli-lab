using Microsoft.Build.Logging.Query.Component;

namespace Microsoft.Build.Logging.Query.Messaging
{
    public abstract class Log
    {
        public string Text { get; }
        public BuildComponent Parent { get; }

        public Log(string text, BuildComponent parent)
        {
            Text = text;
            Parent = parent;
        }
    }
}
