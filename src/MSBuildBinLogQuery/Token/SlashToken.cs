using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class SlashToken : Token, IEquatable<SlashToken>
    {
        public SlashToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SlashToken);
        }

        public bool Equals([AllowNull] SlashToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}