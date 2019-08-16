namespace Microsoft.Build.Logging.Query.Token
{
    public class TargetToken : Token
    {
        public static TargetToken Instance { get; } = new TargetToken();

        private TargetToken() : base()
        {
        }
    }
}