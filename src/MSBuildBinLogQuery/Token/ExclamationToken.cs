namespace Microsoft.Build.Logging.Query.Token
{
    public class ExclamationToken : Token
    {
        public static ExclamationToken Instance { get; } = new ExclamationToken();

        private ExclamationToken() : base()
        {
        }
    }
}