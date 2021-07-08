namespace Microsoft.Build.Logging.Query.Token
{
    public class BeforeToken : Token
    {
        public static BeforeToken Instance { get; } = new BeforeToken();

        private BeforeToken() : base()
        {
        }
    }
}