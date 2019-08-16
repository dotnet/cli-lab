namespace Microsoft.Build.Logging.Query.Token
{
    public class CommaToken : Token
    {
        public static CommaToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommaToken();
                }

                return _instance;
            }
        }

        private static CommaToken _instance;

        private CommaToken() : base()
        {
        }
    }
}