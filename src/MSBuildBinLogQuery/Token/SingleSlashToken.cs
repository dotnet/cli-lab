using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class SingleSlashToken : Token, IEquatable<SingleSlashToken>
    {
        public SingleSlashToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SingleSlashToken);
        }

        public bool Equals([AllowNull] SingleSlashToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(SingleSlashToken).GetHashCode();
        }
    }
}