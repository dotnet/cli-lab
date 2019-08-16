namespace Microsoft.Build.Logging.Query.Token
{
    public class MessageToken : Token
    {
        public static MessageToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MessageToken();
                }

                return _instance;
            }
        }

        private static MessageToken _instance;

        private MessageToken() : base()
        {
        }
    }
}