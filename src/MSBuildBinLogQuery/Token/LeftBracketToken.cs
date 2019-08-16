using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class LeftBracketToken : Token, IEquatable<LeftBracketToken>
    {
        public LeftBracketToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LeftBracketToken);
        }

        public bool Equals([AllowNull] LeftBracketToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(LeftBracketToken).GetHashCode();
        }
    }
}