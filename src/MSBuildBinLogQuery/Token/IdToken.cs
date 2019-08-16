namespace Microsoft.Build.Logging.Query.Token
{
    public class IdToken : Token
    {
        public static IdToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IdToken();
                }

                return _instance;
            }
        }

        private static IdToken _instance;

        private IdToken() : base()
        {
        }
    }
}