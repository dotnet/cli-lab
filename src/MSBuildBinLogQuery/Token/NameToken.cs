using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class NameToken : Token, IEquatable<NameToken>
    {
        public NameToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NameToken);
        }

        public bool Equals([AllowNull] NameToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(NameToken).GetHashCode();
        }
    }
}