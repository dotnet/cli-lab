namespace Microsoft.Build.Logging.Query.Token
{
    public class IdToken : Token
    {
        public static IdToken Instance { get; } = new IdToken();

        private IdToken() : base()
        {
        }
    }
}