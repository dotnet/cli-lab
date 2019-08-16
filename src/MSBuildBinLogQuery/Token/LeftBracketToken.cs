namespace Microsoft.Build.Logging.Query.Token
{
    public class LeftBracketToken : Token
    {
        public static LeftBracketToken Instance { get; } = new LeftBracketToken();

        private LeftBracketToken() : base()
        {
        }
    }
}