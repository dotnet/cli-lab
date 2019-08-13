namespace Microsoft.Build.Logging.Query.Token
{
    public class StringToken : Token
    {
        public string Value { get; }

        public StringToken(string value) : base()
        {
            Value = value;
        }
    }
}