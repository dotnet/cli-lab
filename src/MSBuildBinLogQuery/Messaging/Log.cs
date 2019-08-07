namespace Microsoft.Build.Logging.Query.Messaging
{
    public abstract class Log
    {
        public string Text { get; }

        public Log(string text)
        {
            Text = text;
        }
    }
}
