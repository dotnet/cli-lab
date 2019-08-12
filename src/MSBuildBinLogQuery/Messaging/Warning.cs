namespace Microsoft.Build.Logging.Query.Messaging
{
    public class Warning : Log
    {
        public Warning(string text, Component.Component parent) : base(text, parent)
        {
        }
    }
}