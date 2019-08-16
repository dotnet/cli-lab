namespace Microsoft.Build.Logging.Query.Token
{
    public class WarningToken : Token
    {
        public static WarningToken Instance { get; } = new WarningToken();

        private WarningToken() : base()
        {
        }
    }
}