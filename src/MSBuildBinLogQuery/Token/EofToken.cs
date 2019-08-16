using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class EofToken : Token, IEquatable<EofToken>
    {
        public EofToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EofToken);
        }

        public bool Equals([AllowNull] EofToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(EofToken).GetHashCode();
        }
    }
}