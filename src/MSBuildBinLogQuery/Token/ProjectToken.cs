namespace Microsoft.Build.Logging.Query.Token
{
    public class ProjectToken : Token
    {
        public static ProjectToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProjectToken();
                }

                return _instance;
            }
        }

        private static ProjectToken _instance;

        private ProjectToken() : base()
        {
        }
    }
}