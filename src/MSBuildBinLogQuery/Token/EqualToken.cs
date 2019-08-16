namespace Microsoft.Build.Logging.Query.Token
{
    public class EqualToken : Token
    {
        public static EqualToken Instance { get; } = new EqualToken();

        private EqualToken() : base()
        {
        }
    }
}