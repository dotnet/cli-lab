namespace Microsoft.Build.Logging.Query.Token
{
    public class PathToken : Token
    {
        public static PathToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PathToken();
                }

                return _instance;
            }
        }

        private static PathToken _instance;

        private PathToken() : base()
        {
        }
    }
}