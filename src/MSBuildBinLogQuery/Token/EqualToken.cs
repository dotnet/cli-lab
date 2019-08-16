using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class EqualToken : Token, IEquatable<EqualToken>
    {
        public EqualToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EqualToken);
        }

        public bool Equals([AllowNull] EqualToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(EqualToken).GetHashCode();
        }
    }
}