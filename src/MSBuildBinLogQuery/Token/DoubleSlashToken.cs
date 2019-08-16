namespace Microsoft.Build.Logging.Query.Token
{
    public class DoubleSlashToken : Token
    {
        public static DoubleSlashToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DoubleSlashToken();
                }

                return _instance;
            }
        }

        private static DoubleSlashToken _instance;

        private DoubleSlashToken() : base()
        {
        }
    }
}