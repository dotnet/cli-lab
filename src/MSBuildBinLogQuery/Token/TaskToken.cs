namespace Microsoft.Build.Logging.Query.Token
{
    public class TaskToken : Token
    {
        public static TaskToken Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TaskToken();
                }

                return _instance;
            }
        }

        private static TaskToken _instance;

        private TaskToken() : base()
        {
        }
    }
}