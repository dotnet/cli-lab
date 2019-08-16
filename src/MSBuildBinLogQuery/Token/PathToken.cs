using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public sealed class PathToken : Token, IEquatable<PathToken>
    {
        public PathToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PathToken);
        }

        public bool Equals([AllowNull] PathToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(PathToken).GetHashCode();
        }
    }
}