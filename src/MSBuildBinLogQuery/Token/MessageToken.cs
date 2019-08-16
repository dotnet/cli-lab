namespace Microsoft.Build.Logging.Query.Token
{
    public class MessageToken : Token
    {
        public static MessageToken Instance { get; } = new MessageToken();

        private MessageToken() : base()
        {
        }
    }
}