namespace Microsoft.Build.Logging.Query.Messaging
{
    public class Error : Log
    {
        public Error(string text, Component.Component parent) : base(text, parent)
        {
        }
    }
}