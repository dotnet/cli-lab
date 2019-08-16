namespace Microsoft.Build.Logging.Query.Token
{
    public class NameToken : Token
    {
        public static NameToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NameToken();
                }

                return _instance;
            }
        }

        private static NameToken _instance;

        private NameToken() : base()
        {
        }
    }
}