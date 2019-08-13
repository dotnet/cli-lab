using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class CommaToken : Token, IEquatable<CommaToken>
    {
        public CommaToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommaToken);
        }

        public bool Equals([AllowNull] CommaToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}