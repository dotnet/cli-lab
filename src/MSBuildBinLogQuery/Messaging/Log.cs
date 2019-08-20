using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Messaging
{
    public abstract class Log : IQueryResult
    {
        public string Text { get; }
        public Component.Component Parent { get; }

        public Log(string text, Component.Component parent)
        {
            Text = text;
            Parent = parent;
        }
    }
}