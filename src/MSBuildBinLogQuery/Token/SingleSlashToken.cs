namespace Microsoft.Build.Logging.Query.Token
{
    public class SingleSlashToken : Token
    {
        public static SingleSlashToken Instance { get; } = new SingleSlashToken();

        private SingleSlashToken() : base()
        {
        }
    }
}