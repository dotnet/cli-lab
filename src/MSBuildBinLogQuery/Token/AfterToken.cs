namespace Microsoft.Build.Logging.Query.Token
{
    public class AfterToken : Token
    {
        public static AfterToken Instance { get; } = new AfterToken();

        private AfterToken() : base()
        {
        }
    }
}