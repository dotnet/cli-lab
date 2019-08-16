namespace Microsoft.Build.Logging.Query.Token
{
    public class NameToken : Token
    {
        public static NameToken Instance { get; } = new NameToken();

        private NameToken() : base()
        {
        }
    }
}