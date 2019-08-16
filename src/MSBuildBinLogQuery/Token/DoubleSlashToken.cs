namespace Microsoft.Build.Logging.Query.Token
{
    public class DoubleSlashToken : Token
    {
        public static DoubleSlashToken Instance { get; } = new DoubleSlashToken();

        private DoubleSlashToken() : base()
        {
        }
    }
}