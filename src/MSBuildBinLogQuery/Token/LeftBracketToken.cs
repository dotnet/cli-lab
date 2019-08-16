namespace Microsoft.Build.Logging.Query.Token
{
    public class LeftBracketToken : Token
    {
        public static LeftBracketToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LeftBracketToken();
                }

                return _instance;
            }
        }

        private static LeftBracketToken _instance;

        private LeftBracketToken() : base()
        {
        }
    }
}