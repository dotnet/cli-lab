using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class ErrorToken : Token, IEquatable<ErrorToken>
    {
        public ErrorToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ErrorToken);
        }

        public bool Equals([AllowNull] ErrorToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(ErrorToken).GetHashCode();
        }
    }
}