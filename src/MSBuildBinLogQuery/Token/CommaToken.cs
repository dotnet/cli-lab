namespace Microsoft.Build.Logging.Query.Token
{
    public class CommaToken : Token
    {
        public static CommaToken Instance { get; } = new CommaToken();

        private CommaToken() : base()
        {
        }
    }
}