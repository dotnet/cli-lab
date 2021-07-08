namespace Microsoft.Build.Logging.Query.Token
{
    public class DependOnToken : Token
    {
        public static DependOnToken Instance { get; } = new DependOnToken();

        private DependOnToken() : base()
        {
        }
    }
}