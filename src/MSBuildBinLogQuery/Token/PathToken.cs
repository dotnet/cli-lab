namespace Microsoft.Build.Logging.Query.Token
{
    public class PathToken : Token
    {
        public static PathToken Instance { get; } = new PathToken();

        private PathToken() : base()
        {
        }
    }
}