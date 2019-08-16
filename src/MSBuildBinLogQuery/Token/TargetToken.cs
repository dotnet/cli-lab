using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class TargetToken : Token, IEquatable<TargetToken>
    {
        public TargetToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TargetToken);
        }

        public bool Equals([AllowNull] TargetToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(TargetToken).GetHashCode();
        }
    }
}