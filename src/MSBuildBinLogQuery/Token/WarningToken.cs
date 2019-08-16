using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class WarningToken : Token, IEquatable<WarningToken>
    {
        public WarningToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WarningToken);
        }

        public bool Equals([AllowNull] WarningToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(WarningToken).GetHashCode();
        }
    }
}