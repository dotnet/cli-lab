namespace Microsoft.Build.Logging.Query.Token
{
    public class TaskToken : Token
    {
        public static TaskToken Instance { get; } = new TaskToken();

        private TaskToken() : base()
        {
        }
    }
}