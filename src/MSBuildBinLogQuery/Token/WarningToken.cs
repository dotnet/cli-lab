namespace Microsoft.Build.Logging.Query.Token
{
    public class WarningToken : Token
    {
        public static WarningToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WarningToken();
                }

                return _instance;
            }
        }

        private static WarningToken _instance;

        private WarningToken() : base()
        {
        }
    }
}