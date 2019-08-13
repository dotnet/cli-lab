using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class RightBracketToken : Token, IEquatable<RightBracketToken>
    {
        public RightBracketToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RightBracketToken);
        }

        public bool Equals([AllowNull] RightBracketToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}