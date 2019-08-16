namespace Microsoft.Build.Logging.Query.Token
{
    public class SingleSlashToken : Token
    {
        public static SingleSlashToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SingleSlashToken();
                }

                return _instance;
            }
        }

        private static SingleSlashToken _instance;

        private SingleSlashToken() : base()
        {
        }
    }
}