namespace Microsoft.Build.Logging.Query.Token
{
    public class RightBracketToken : Token
    {
        public static RightBracketToken Instance { get; } = new RightBracketToken();

        private RightBracketToken() : base()
        {
        }
    }
}