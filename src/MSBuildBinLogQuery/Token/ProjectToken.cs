using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class ProjectToken : Token, IEquatable<ProjectToken>
    {
        public ProjectToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectToken);
        }

        public bool Equals([AllowNull] ProjectToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}