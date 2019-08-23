namespace Microsoft.Build.Logging.Query.Token
{
    public class AsteriskToken : Token
    {
        public static AsteriskToken Instance { get; } = new AsteriskToken();

        private AsteriskToken() : base()
        {
        }
    }
}