namespace Microsoft.Build.Logging.Query.Result
{
    public abstract class Log : IQueryResult
    {
        public string Text { get; }
        public Component Parent { get; }

        public Log(string text, Component parent)
        {
            Text = text;
            Parent = parent;
        }
    }
}