namespace Microsoft.Build.Logging.Query.Token
{
    public class ErrorToken : Token
    {
        public static ErrorToken Instance { get; } = new ErrorToken();

        private ErrorToken() : base()
        {
        }
    }
}