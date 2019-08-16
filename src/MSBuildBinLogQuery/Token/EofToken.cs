namespace Microsoft.Build.Logging.Query.Token
{
    public class EofToken : Token
    {
        public static EofToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EofToken();
                }

                return _instance;
            }
        }

        private static EofToken _instance;

        private EofToken() : base()
        {
        }
    }
}