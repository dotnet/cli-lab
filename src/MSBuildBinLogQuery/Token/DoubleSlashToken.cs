using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class DoubleSlashToken : Token, IEquatable<DoubleSlashToken>
    {
        public DoubleSlashToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DoubleSlashToken);
        }

        public bool Equals([AllowNull] DoubleSlashToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(DoubleSlashToken).GetHashCode();
        }
    }
}