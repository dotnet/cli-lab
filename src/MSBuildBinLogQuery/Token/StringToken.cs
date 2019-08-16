using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class StringToken : Token, IEquatable<StringToken>
    {
        public string Value { get; }

        public StringToken(string value) : base()
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StringToken);
        }

        public bool Equals([AllowNull] StringToken other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}