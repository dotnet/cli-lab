using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class IntegerToken : Token, IEquatable<IntegerToken>
    {
        public int Value { get; }

        public IntegerToken(int value) : base()
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IntegerToken);
        }

        public bool Equals([AllowNull] IntegerToken other)
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