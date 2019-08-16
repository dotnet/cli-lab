namespace Microsoft.Build.Logging.Query.Token
{
    public class TargetToken : Token
    {
        public static TargetToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TargetToken();
                }

                return _instance;
            }
        }

        private static TargetToken _instance;

        private TargetToken() : base()
        {
        }
    }
}