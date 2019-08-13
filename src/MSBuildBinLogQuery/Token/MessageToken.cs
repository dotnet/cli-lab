using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class MessageToken : Token, IEquatable<MessageToken>
    {
        public MessageToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MessageToken);
        }

        public bool Equals([AllowNull] MessageToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}