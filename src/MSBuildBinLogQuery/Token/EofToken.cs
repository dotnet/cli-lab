namespace Microsoft.Build.Logging.Query.Token
{
    public class EofToken : Token
    {
        public static EofToken Instance { get; } = new EofToken();

        private EofToken() : base()
        {
        }
    }
}