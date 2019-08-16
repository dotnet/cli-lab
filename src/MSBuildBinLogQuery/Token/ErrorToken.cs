namespace Microsoft.Build.Logging.Query.Token
{
    public class ErrorToken : Token
    {
        public static ErrorToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ErrorToken();
                }

                return _instance;
            }
        }

        private static ErrorToken _instance;

        private ErrorToken() : base()
        {
        }
    }
}