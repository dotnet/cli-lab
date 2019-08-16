namespace Microsoft.Build.Logging.Query.Token
{
    public class EqualToken : Token
    {
        public static EqualToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EqualToken();
                }

                return _instance;
            }
        }

        private static EqualToken _instance;

        private EqualToken() : base()
        {
        }
    }
}