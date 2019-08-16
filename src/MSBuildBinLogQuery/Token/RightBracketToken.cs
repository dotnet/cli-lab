namespace Microsoft.Build.Logging.Query.Token
{
    public class RightBracketToken : Token
    {
        public static RightBracketToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RightBracketToken();
                }

                return _instance;
            }
        }

        private static RightBracketToken _instance;

        private RightBracketToken() : base()
        {
        }
    }
}