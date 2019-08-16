using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class IdToken : Token, IEquatable<IdToken>
    {
        public IdToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdToken);
        }

        public bool Equals([AllowNull] IdToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(IdToken).GetHashCode();
        }
    }
}