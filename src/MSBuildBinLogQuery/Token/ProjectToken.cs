namespace Microsoft.Build.Logging.Query.Token
{
    public class ProjectToken : Token
    {
        public static ProjectToken Instance { get; } = new ProjectToken();

        private ProjectToken() : base()
        {
        }
    }
}